# Heroku

## Prerequisites

* Heroku CLI - https://devcenter.heroku.com/articles/heroku-cli

## Authentication

```
heroku login
```

```
heroku container:login
```

## Create an app

```
// Create app
heroku apps:create nome-do-seu-app

// Push container
cd src/
docker build -f .\PlanningPoker.Bff\Dockerfile -t registry.heroku.com/nome-do-seu-app/web .
docker push registry.heroku.com/nome-do-seu-app/web

// Release an image
heroku container:release web -a nome-do-seu-app

// Logs
heroku logs -a nome-do-seu-app
```