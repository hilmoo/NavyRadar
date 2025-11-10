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
    ADD FOREIGN KEY ("sail_id") REFERENCES "sail" ("id"); -- admin:admin
INSERT INTO account (username, password, email, role)
VALUES ('admin', '$2a$11$paZlOeBJKaEgc8Ae9EoWPO3f4v6Az0bQxMuoTGRkKRa7MBRfUzwx6', 'admin@example.com', 'Admin');

-- user:user
INSERT INTO account (username, password, email, role)
VALUES ('user', '$2a$11$4AqOgGubsQRm/foTtHfRUuc/2qB6N6QN4AIPPeHoLPSvXLgXUif/u', 'user@example.com', 'User');

-- captain:captain
INSERT INTO account (username, password, email, role)
VALUES ('captain', '$2a$11$mrwVD6gD2ik57KYcVKeXEOviL2XomVA0X0E7Aa6.vmbTfWSskLV72', 'captain@example.com', 'Captain');

INSERT INTO captain (account_id, first_name, last_name, license_number)
VALUES (3, 'James', 'Kirk', 'NCC-1701'); INSERT INTO port (name, country_code, location)
VALUES ('Pelabuhan Ketapang', 'ID', '(114.399697,-8.147557)'),
       ('Pelabuhan Gilimanuk', 'ID', '(114.437745,-8.161871)'),
       ('Pelabuhan Tanjung Perak', 'ID', '(112.732778,-7.196667)'); INSERT INTO ship (imo_number, mmsi_number, name, type)
VALUES ('9000001', '3000001', 'MV Java Star', 'Cargo Vessels'),
       ('9000002', '3000002', 'KM Bali Express', 'Tankers'),
       ('9000003', '3000003', 'MT Ocean Voyager', 'Fishing'); INSERT INTO sail (ship_id, captain_id, origin_port_id, destination_port_id, status)
VALUES (1, 1, 1, 2, 'Sailing'),
       (2, 1, 2, 1, 'Sailing'),
       (3, 1, 1, 2, 'Sailing'); INSERT INTO position_history (sail_id, coordinates, speed_knots, heading_degrees, timestamp)
VALUES (1, point(112.7, -7.0), 12.5, 47, NOW()),
       (2, point(114.4, -8.3), 20.0, 210, NOW()),
       (3, point(114.0, -7.5), 10.2, 185, NOW()); 
