# Inventory Management Frontend

A frontend application for the Inventory Management system built with Vue.js and TypeScript.

## Getting Started

This guide explains how to set up and run the frontend project in **development mode**.

### Prerequisites

- [Bun](https://bun.sh/) runtime installed

### Installation

Execute the following command in the root directory for installing dependencies:

```bash
bun install
```

### Configuration Setup

1. Create a new file named `.env.development` in the root directory
2. Copy the contents from `.env.example`
3. Update the environment variables according to your setup (the default ones are fine if you did not change anything in the back-end configuration, but you have to [create GitHub application](https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/creating-an-oauth-app) and pass credentials to it)

### Running the Application

Start the development server:

```bash
bun dev --port 4000
```

The application should now be running at `http://localhost:4000`

## Production Deployment

To deploy the application in production:

1. Create `.env.production` file
2. Set the production environment variables
3. Build the project:

```bash
bun run build
```

4. The built files will be available in the `dist` directory
