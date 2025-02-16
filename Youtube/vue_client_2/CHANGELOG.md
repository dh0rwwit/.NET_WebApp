This file explains how Visual Studio created the project.

The following tools were used to generate this project:
- create-vite

The following steps were used to generate this project:
- Create vue project with create-vite: `npm init --yes vue@latest vue_client_2 -- --eslint `.
- Create project file (`vue_client_2.esproj`).
- Create `launch.json` to enable debugging.
- Create `nuget.config` to specify location of the JavaScript Project System SDK (which is used in the first line in `vue_client_2.esproj`).
- Add project to solution.
- Write this file.
