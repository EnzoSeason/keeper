# keeper

A financial reporting tool

## Architecture

![archi-1](./img/archi-1.png)
[Source](https://miro.com/app/board/uXjVMWXddkA=/)

There are 2 services:

- Identity Service: It manages the user. Authentication is not included.

- Reporting Service: It's composed by 2 parts

  - **Reporting Controller**: It has 2 endpoint

    - `POST`: It receives the data sources from the web app, clean up data, and write them into database.It returns nothing if success
    
    - `POST`: It receives the command that asks the App to read the data from database, create the report, and save the results into database

    - `GET`: It returns the reporting results.

  - **Reporting Service**:

    1. It maps the requests to the internal data structures.

    2. It creates the reports based on the local **user reporting config json**. This config file is attached to a user by his `user_id`

    3. It saves the orginal data and reporting data into the database.
    
## Usage

To run the project in "Production" on the local machine

```code
docker compose up
```

## Development

Run the project locally. Please lanuch **MongoDB** on your local machine.
  
