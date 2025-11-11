--liquibase formatted sql

--changeset developer:001-create-users-table
--comment: Create Users table with basic user information

CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID NOT NULL,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "PhoneNumber" VARCHAR(20),
    "Rating" INT,
    "CreatedAt" TIMESTAMP NOT NULL,
    "UpdatedAt" TIMESTAMP,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

--rollback DROP TABLE IF EXISTS "Users";

--changeset developer:001-create-users-email-index
--comment: Create unique index on Email column for Users table

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");

--rollback DROP INDEX IF EXISTS "IX_Users_Email";

