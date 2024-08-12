# BackendChallenge

Steps to run this project

- Open a terminal and go into the project folder
- Run "docker-compose up -d" to start the containers
- At this point, we should have: a postgres database, a rabbitmq instance and the api running.
- Access http://localhost:5263/swagger/index.html to check api endpoints
- Users seeded:

  - role: admin
    username: admin
    password: Pass123!

  - role: deliveryman
    username: deliveryman
    password: Pass123!

- Use swagger UI to authenticate your resquests, digit: Bearer {token}
- Azure storage was used to store cnh images
- Keys are visible, use carefully and after your review I'll delete the resources.
- docker-compose contains all the credentials to access the postgresql, use pgAdmin to access the database to check the ids if needed.

This project is using vertical slices to organize the code.

Next steps if I had more time:

- Integration tests with testcontainers
- Cache with redis
- More validations
- More unit tests
- Refactor to abstract all ApplicationDbContext calls using a repository
