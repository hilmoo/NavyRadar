CREATE TABLE "account"
(
    "id"       integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "username" text UNIQUE NOT NULL,
    "password" text        NOT NULL,
    "email"    text UNIQUE NOT NULL,
    "role"     text        NOT NULL
);

CREATE TABLE "captain"
(
    "id"             integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "account_id"     integer UNIQUE NOT NULL,
    "first_name"     text           NOT NULL,
    "last_name"      text           NOT NULL,
    "license_number" text UNIQUE
);

CREATE TABLE "port"
(
    "id"           integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "name"         text  NOT NULL,
    "country_code" char(2),
    "location"     point NOT NULL
);

CREATE TABLE "ship"
(
    "id"             integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "imo_number"     text UNIQUE NOT NULL,
    "mmsi_number"    text UNIQUE,
    "name"           text        NOT NULL,
    "type"           text,
    "year_build"     integer,
    "length_overall" integer,
    "gross_tonnage"  integer
);

CREATE TABLE "sail"
(
    "id"                  integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "ship_id"             integer NOT NULL,
    "captain_id"          integer NOT NULL,
    "origin_port_id"      integer NOT NULL,
    "destination_port_id" integer NOT NULL,
    "status"              text    NOT NULL,
    "departure_time"      timestamp,
    "arrival_time"        timestamp,
    "total_distance_nm"   numeric,
    "average_speed_knots" numeric,
    "max_speed_knots"     numeric
);

CREATE TABLE "position_history"
(
    "id"              integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "sail_id"         integer   NOT NULL,
    "coordinates"     point     NOT NULL,
    "speed_knots"     decimal(4, 1),
    "heading_degrees" smallint,
    "timestamp"       timestamp NOT NULL
);

ALTER TABLE "captain"
    ADD FOREIGN KEY ("account_id") REFERENCES "account" ("id");

ALTER TABLE "sail"
    ADD FOREIGN KEY ("ship_id") REFERENCES "ship" ("id");

ALTER TABLE "sail"
    ADD FOREIGN KEY ("captain_id") REFERENCES "captain" ("id");

ALTER TABLE "sail"
    ADD FOREIGN KEY ("origin_port_id") REFERENCES "port" ("id");

ALTER TABLE "sail"
    ADD FOREIGN KEY ("destination_port_id") REFERENCES "port" ("id");

ALTER TABLE "position_history"
    ADD FOREIGN KEY ("sail_id") REFERENCES "sail" ("id");
