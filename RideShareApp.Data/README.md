# RideShareApp.Data

This project contains Entity Framework Core entities and database context for the RideShare application.

## Structure

- **Entities/**: EF Core entity classes
- **DbContext/**: Database context configuration
- **liquibase/**: Liquibase migration files

## Liquibase Setup

This project uses Liquibase for database migrations. Liquibase allows you to version control your database schema changes.

### Prerequisites

1. Install Liquibase CLI:
   - Download from: https://www.liquibase.org/download
   - Or use Homebrew: `brew install liquibase`
   - Or use Chocolatey: `choco install liquibase`

2. Ensure PostgreSQL is running (via Docker or local installation)

3. PostgreSQL JDBC Driver:
   - The driver is already included in the `lib/` folder
   - If you need to update it, download from: https://jdbc.postgresql.org/download/

### Running Migrations

1. Navigate to the `RideShareApp.Data` directory:
   ```bash
   cd RideShareApp.Data
   ```

2. Run Liquibase update:
   ```bash
   liquibase --defaults-file=liquibase.properties update
   ```

### Creating New Migrations

1. Create a new SQL changelog file in `liquibase/changelog/` (e.g., `002-add-rides-table.sql`)
   
   Use the following format:
   ```sql
   --liquibase formatted sql
   
   --changeset developer:002-add-rides-table
   --comment: Description of what this migration does
   
   CREATE TABLE IF NOT EXISTS "Rides" (
       -- your table definition here
   );
   
   --rollback DROP TABLE IF EXISTS "Rides";
   ```

2. Add the new changelog to `master-changelog.xml`:
   ```xml
   <include file="changelog/002-add-rides-table.sql"/>
   ```

3. Run the migration using the command above

### Liquibase Commands

- **Update database**: `liquibase --defaults-file=liquibase.properties update`
- **Rollback last change**: `liquibase --defaults-file=liquibase.properties rollback-count 1`
- **Check status**: `liquibase --defaults-file=liquibase.properties status`
- **Generate SQL (dry run)**: `liquibase --defaults-file=liquibase.properties update-sql`

### Configuration

The `liquibase.properties` file contains the database connection settings and classpath to the PostgreSQL JDBC driver. Update it if your database configuration differs from the defaults.

**Note:** If you encounter driver issues, you can also specify the classpath on the command line:
```bash
liquibase --defaults-file=liquibase.properties --classpath=lib/postgresql.jar update
```

