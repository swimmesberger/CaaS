DROP DATABASE IF EXISTS caas;
DROP USER IF EXISTS caas;

CREATE DATABASE caas;
CREATE USER caas WITH ENCRYPTED PASSWORD 'caas';
GRANT ALL PRIVILEGES ON DATABASE caas to caas;