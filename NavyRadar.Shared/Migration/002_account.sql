-- admin:admin
INSERT INTO account (username, password, email, role)
VALUES ('admin', '$2a$11$paZlOeBJKaEgc8Ae9EoWPO3f4v6Az0bQxMuoTGRkKRa7MBRfUzwx6', 'admin@example.com', 'Admin');

-- user:user
INSERT INTO account (username, password, email, role)
VALUES ('user', '$2a$11$4AqOgGubsQRm/foTtHfRUuc/2qB6N6QN4AIPPeHoLPSvXLgXUif/u', 'user@example.com', 'User');

-- captain:captain
INSERT INTO account (username, password, email, role)
VALUES ('captain', '$2a$11$mrwVD6gD2ik57KYcVKeXEOviL2XomVA0X0E7Aa6.vmbTfWSskLV72', 'captain@example.com', 'Captain');

INSERT INTO captain (account_id, first_name, last_name, license_number)
VALUES (3, 'James', 'Kirk', 'NCC-1701');