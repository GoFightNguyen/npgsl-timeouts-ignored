# npgsl-timeouts-ignored
An experiment to determine where a PostgreSQL timeout is being ignored

## Is the problem `PostgreSQL`?
No, the problem is not `PostgreSQL`.  
Here is how to prove this:

- Use `docker-compose` to bring up `PostgreSQL`: `docker-compose up`

- Exec into the `PostgreSQL` container: `docker exec -it npgsl-timeouts-ignored_postgres_1 bash`

- Enter the psql interactive terminal: `psql --username=postgres --password`
  - When prompted for the password, enter `postgres`

- Set the `statement_timeout` to 1s: `set statement_timeout to 1000;`
- Execute: `select pg_sleep(3)::TEXT;`  
- An error occurs: `ERROR:  canceling statement due to statement timeout`