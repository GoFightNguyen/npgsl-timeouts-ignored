# npgsql-timeouts-ignored
An experiment to determine where a PostgreSQL timeout is being ignored

## Is the problem `Npgsql`?
It depends, `Npgsql` does not respect a `CommandTimeout` at the connection level when using async methods.

Here is how to prove this (note: based on executing in Linux):

- `docker-compose up`
- `cd Tests`
- `dotnet test`

There are 4 tests with these results:
| async | timeout level | timeout respected |
| --- | --- | --- |
| False | Connection | True |
| True | Connection | False |
| False | Statement | True |
| True | Statement | True |

### Interacting with `PostgreSQL`
After `docker-compose up`:
- Exec into the `PostgreSQL` container: `docker exec -it npgsl-timeouts-ignored_postgres_1 bash`
- Enter the psql interactive terminal: `psql --username=postgres --password`
  - When prompted for the password, enter `postgres`