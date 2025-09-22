# Overview

This work is for the Pre-hire Developer Coding Project given to DJ Yuhn by CivicPlus.

# PreRequisites

The following prerequisites are needed in order to run this project.

1. NPM installed
2. Docker installed

# Development Run Steps

To run this project the following steps must be performed:

1. Navigate to the `appsettings.Development.json` file.
    1. Replace the ClientId configuration value of `CLIENT_ID` with a valid client ID
    2. Replace the ClientSecret configuration value of `CLIENT_SECRET` with a valid client secret
2. Run the make step `start-dev`
3. Navigate to http://localhost:5198/

# Troubleshooting

If an issue arises and the application cannot be run consider the following.

1. Modify the host ports specified in the `compose.yaml` file for each service.
    * For instance, if port `5198` is unavailable, modify the client service ports to be: `- "####-5198"`, where `####`
      is some available port.
        * Update the `appsettings.Development.json` file client app's port to be the available port.
        * Update the file `client/config.json` to have the config key `vitePort` have the value of the available port.