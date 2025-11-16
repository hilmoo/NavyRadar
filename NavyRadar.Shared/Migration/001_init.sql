CREATE EXTENSION IF NOT EXISTS cube;
CREATE EXTENSION IF NOT EXISTS earthdistance;

CREATE TYPE account_role AS ENUM (
    'User',
    'Admin',
    'Captain'
    );

CREATE TABLE IF NOT EXISTS "account"
(
    "id"       integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "username" text UNIQUE  NOT NULL,
    "password" text         NOT NULL,
    "email"    text UNIQUE  NOT NULL,
    "role"     account_role NOT NULL
);


CREATE TABLE IF NOT EXISTS "captain"
(
    "id"             integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "account_id"     integer UNIQUE NOT NULL,
    "first_name"     text           NOT NULL,
    "last_name"      text           NOT NULL,
    "license_number" text UNIQUE    NOT NULL
);

CREATE TABLE IF NOT EXISTS "port"
(
    "id"           integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "name"         text UNIQUE NOT NULL,
    "country_code" char(2)     NOT NULL,
    "location"     point       NOT NULL
);

CREATE TYPE ship_type AS ENUM (
    'Cargo Vessels',
    'Tankers',
    'Passenger Vessels',
    'High Speed Craft',
    'Tugs & Special Craft',
    'Fishing',
    'Pleasure Craft',
    'Navigation Aids',
    'Unspecified Ships'
    );

CREATE TABLE IF NOT EXISTS "ship"
(
    "id"             integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "imo_number"     text UNIQUE NOT NULL,
    "mmsi_number"    text UNIQUE NOT NULL,
    "name"           text        NOT NULL,
    "type"           ship_type   NOT NULL,
    "year_build"     integer     NOT NULL,
    "length_overall" integer     NOT NULL,
    "gross_tonnage"  integer     NOT NULL
);

CREATE TYPE sail_status AS ENUM (
    'Sailing',
    'Docked',
    'Finished',
    'Cancelled'
    );

CREATE TABLE IF NOT EXISTS "sail"
(
    "id"                  integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "ship_id"             integer     NOT NULL,
    "captain_id"          integer     NOT NULL,
    "origin_port_id"      integer     NOT NULL,
    "destination_port_id" integer     NOT NULL,
    "status"              sail_status NOT NULL,
    "departure_time"      timestamp   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "arrival_time"        timestamp,
    "total_distance_nm"   numeric,
    "average_speed_knots" numeric,
    "max_speed_knots"     numeric
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_unique_active_captain"
    ON "sail" ("captain_id")
    WHERE ("arrival_time" IS NULL);

CREATE TABLE IF NOT EXISTS "position_history"
(
    "id"              integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "sail_id"         integer       NOT NULL,
    "coordinates"     point         NOT NULL,
    "speed_knots"     decimal(4, 1) NOT NULL,
    "heading_degrees" smallint      NOT NULL,
    "timestamp"       timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE captain
    ADD CONSTRAINT fk_captain_account
        FOREIGN KEY (account_id) REFERENCES account (id)
            ON DELETE CASCADE
            ON UPDATE CASCADE;

ALTER TABLE sail
    ADD CONSTRAINT fk_sail_ship
        FOREIGN KEY (ship_id) REFERENCES ship (id)
            ON DELETE RESTRICT
            ON UPDATE CASCADE;

ALTER TABLE sail
    ADD CONSTRAINT fk_sail_captain
        FOREIGN KEY (captain_id) REFERENCES captain (id)
            ON DELETE RESTRICT
            ON UPDATE CASCADE;

ALTER TABLE sail
    ADD CONSTRAINT fk_sail_origin_port
        FOREIGN KEY (origin_port_id) REFERENCES port (id)
            ON DELETE RESTRICT
            ON UPDATE CASCADE;

ALTER TABLE sail
    ADD CONSTRAINT fk_sail_destination_port
        FOREIGN KEY (destination_port_id) REFERENCES port (id)
            ON DELETE RESTRICT
            ON UPDATE CASCADE;

ALTER TABLE position_history
    ADD CONSTRAINT fk_poshist_sail
        FOREIGN KEY (sail_id) REFERENCES sail (id)
            ON DELETE CASCADE
            ON UPDATE CASCADE;

CREATE OR REPLACE FUNCTION prevent_captain_role_change()
    RETURNS TRIGGER AS
$$
BEGIN
    IF EXISTS(SELECT 1 FROM "captain" WHERE "account_id" = NEW.id) THEN
        IF NEW.role != 'Captain' THEN
            RAISE EXCEPTION 'Cannot change role: Account % is a captain and must have the ''Captain'' role.', NEW.username;
        END IF;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER "trg_prevent_captain_role_change"
    BEFORE UPDATE
    ON "account"
    FOR EACH ROW
    WHEN (OLD.role IS DISTINCT FROM NEW.role)
EXECUTE FUNCTION prevent_captain_role_change();

CREATE OR REPLACE FUNCTION create_initial_position()
    RETURNS TRIGGER AS
$$
BEGIN
    INSERT INTO "position_history" (sail_id, coordinates, speed_knots, heading_degrees, "timestamp")
    SELECT NEW.id,
           p."location",
           0.0,
           0,
           NEW.departure_time
    FROM "port" AS p
    WHERE p."id" = NEW.origin_port_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER "trg_create_initial_position"
    AFTER INSERT
    ON "sail"
    FOR EACH ROW
EXECUTE FUNCTION create_initial_position();