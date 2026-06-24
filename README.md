[![Devart MCP Server for SQLite](https://github.com/devart-ai-connectivity/.github/blob/main/assets/cover-banner-mcp-server-for-sqlite.webp?raw=true)](https://www.devart.com/mcp/)

# Devart MCP Server for SQLite

Devart MCP Server for SQLite enables AI clients to interact with your data through a secure server running in your environment. It turns a regular AI chat into a practical way to work with real-world business data — and it is faster than conventional export or manual querying.

## Key benefits

Devart MCP Server for SQLite allows you to:

* Work with data intuitively through natural language.
* Retrieve the required data for analysis within minutes.
* Report on your data faster with AI-powered assistance.
* Minimize manual data handling and integration maintenance.

## How it works

Devart MCP Server for SQLite helps AI clients communicate directly with SQLite databases using natural-language prompts. It translates AI requests into structured queries, executes them through Devart connectivity drivers, and returns clean, structured results for seamless AI-powered data access.

![Devart MCP Server architecture](https://github.com/devart-ai-connectivity/.github/blob/main/assets/how_mcp_works_single.webp?raw=true)

## Quick start

To get started with Devart MCP Server for SQLite:

1\. [Download](https://www.devart.com/odbc/sqlite/download.html) and [install](https://docs.devart.com/odbc-driver-for-sqlite/installation/) Devart ODBC Driver for SQLite.

2\. [Download](https://www.devart.com/mcp/download.html) and [install](https://docs.devart.com/mcp/installation.html) Devart MCP Server for SQLite.

3\. In Devart MCP Server for SQLite, [configure your data connection and integration settings](https://docs.devart.com/mcp/connection-configuration.html).

![Devart MCP Server configuration](https://github.com/devart-ai-connectivity/.github/blob/main/assets/mcp-servers-gui.webp?raw=true)

4\. Run your first natural-language query.

[![Need an MCP Server for multiple data sources?](https://github.com/devart-ai-connectivity/.github/blob/main/assets/need-mcp-server-universal.webp?raw=true)](https://www.devart.com/mcp/universal/)


## Manual installation and configuration 

**Prerequisites** 

Before building and running Devart MCP Server for SQLite, ensure the following components are installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* **ADO.NET connection** — **Devart.AI.McpServer.AdoNet.Sqlite.csproj** Devart dotConnect for SQLite (installed automatically via NuGet during build)
* **ODBC connection** — **Devart.AI.McpServer.Odbc.Sqlite.csproj** [Devart ODBC Driver for SQLite](https://www.devart.com/odbc/sqlite/download.html) (requires manual download and installation)

**Step 1: Clone the repository**

Clone the project repository and navigate to the project directory:

1\. Open **Command Prompt**.

2\. Enter the following command:

```cmd
git clone https://github.com/devart-ai-connectivity/devart-mcp-server-sqlite.git
cd devart-mcp-server-sqlite
```

**Step 2: Build the MCP Server from source**

You can build Devart MCP Server for SQLite from source using either of the supported database connectivity technologies: ADO.NET or ODBC.

* To build the MCP server with ADO.NET, run the following command:

```cmd
dotnet publish Devart.AI.McpServer.AdoNet/Devart.AI.McpServer.AdoNet.Sqlite/Devart.AI.McpServer.AdoNet.Sqlite.csproj -c ReleaseSqlite /p:TargetFramework=net8.0
```
The Devart dotConnect for SQLite NuGet package is downloaded and restored automatically.

* To build the MCP server with ODBC, select the command based on the bitness of your data source.

For 64-bit data source, run the following command:

```cmd
dotnet publish Devart.AI.McpServer.Odbc/Devart.AI.McpServer.Odbc.Sqlite/Devart.AI.McpServer.Odbc.Sqlite.csproj -c ReleaseSqlite -r "win-x64" /p:TargetFramework=net8.0
```

For 32-bit data source, run the following command:

```cmd
dotnet publish Devart.AI.McpServer.Odbc/Devart.AI.McpServer.Odbc.Sqlite/Devart.AI.McpServer.Odbc.Sqlite.csproj -c ReleaseSqlite -r "win-x86" /p:TargetFramework=net8.0
```
>**Note**
>
>The target platform must match the bitness of your ODBC data source.

**Step 3: Configure the database connection for the MCP Server**

1\. Create an `mcpserver.json` configuration file in the directory containing the built MCP Server executable.

2\. In the file, configure the database connection.

* Configure a connection with ADO.NET.

Add the following configuration to the `mcpserver.json` file:

```json
{
  "Connections": [
    {
      "Name": "my_sqlite",
      "ConnectionString": "Server=localhost;User Id=sqlite;Password=your_password;Database=your_database;",
      "ProtocolType": "stdio"
    }
  ]
}
```

* Configure a connection with ODBC.

Add the following configuration to the `mcpserver.json` file:

```json
{
  "Connections": [
    {
      "Name": "my_sqlite",
      "DsnName": "your_dsn_name",
      "ProtocolType": "stdio"
    }
  ]
}
```

* Configure a connection with ODBC using a connection string.

Add the following configuration to the `mcpserver.json` file:

```json
{
  "Connections": [
    {
      "Name": "my_sqlite",
      "ConnectionString": "Driver={Devart ODBC Driver for SQLite};Server=localhost;User ID=sqlite;Password=your_password;Database=your_database;",
      "ProtocolType": "stdio"
    }
  ]
}
```
where:

* `Name` — The connection name.

* `ConnectionString` (applies to ADO.NET) — A database-specific connection string used to establish a connection to the target database.

* `DsnName` (applies to ODBC) — The name of your data source.

* `ProtocolType` — A transport protocol. The possible options are: `stdio` or `http`.

* `HttpPort` (required if `ProtocolType` is set to `http`) — The port number for the `http` protocol. 

**Step 4: Run the MCP server**

After you configure the MCP Server, you can start it. 

>**Note**
>
>This step is required only when `ProtocolType` is configured as `http`. If you use the `stdio` transport protocol, your AI client starts the server automatically.

* To start the server with ADO.NET, run the following command:

```cmd
Devart.AI.McpServer.AdoNet.Sqlite.exe run my_sqlite
```

* To start the server with ODBC, run the following command:

```cmd
Devart.AI.McpServer.Odbc.Sqlite.exe run my_sqlite
```

where `my_sqlite` is the name of the ODBC connection.

**Step 5: Integrate with Claude Desktop**

1\. Open `claude_desktop_config.json`, the Claude configuration file.

>**Tip**
>
>If you can't locate the configuration file, it may not exist yet. To create it, open Claude Desktop and navigate to **File** > **Settings** > **Developer**, then click **Edit Config**. The folder with the `claude_desktop_config.json` file opens.

2\. Add one of the following objects, depending on the transport protocol used by MCP Server:

* STDIO

```json
{
  "mcpServers": {
    "devart": {
      "command": "C:\\path\\to\\Devart.AI.McpServer.AdoNet.Sqlite.exe",
      "args": [
        "run",
        "my_sqlite"
      ]
    }
  }
}
```

 where:

  * `devart` is the connector name that will appear in Claude Desktop.
  * `C:\\path\\to\\Devart.AI.McpServer.AdoNet.Sqlite.exe` is the path to the executable file. For an ODBC connection, use `Devart.AI.McpServer.Odbc.Sqlite.exe`.
  * `my_sqlite` is the connection name specified in the `mcpserver.json` configuration file.

* **HTTP**

  ```json
    "mcpServers": {
      "devart": {
        "command": "npx",
        "args": [
          "-y",
          "mcp-remote",
          "http://localhost:5000/sse"
        ]
      }
    }
  ```

  where:

  * `devart` is the connector name that will appear in Claude Desktop.
  * `5000` is the MCP Server listening port.

3\. Save the file.

4\. Restart Claude Desktop.

Devart MCP Server for SQLite is now integrated with Claude, and **devart** appears in the Claude Desktop app in **Customize** > **Connectors**.

You can also [integrate](https://docs.devart.com/mcp/ai-integration/) Devart MCP Server for SQLite with other AI clients such as Cline, Codex, Cursor, Visual Studio Code, Windsurf, Zed.

## Supported clients

Devart MCP Server for SQLite supports integration with the following AI clients: 
 
* Claude Desktop
* Visual Studio Code
* Cursor
* Codex
* Windsurf
* Cline
* Zed
* ...and other MCP-compatible AI clients

## Typical use cases

Devart MCP Server for SQLite is a practical fit for teams working with SQLite as their primary data source.

* **Mobile and desktop app data analysis**  
  Query SQLite databases embedded in mobile applications or desktop tools to analyze user behavior, local state, cached data, and application logs.

* **Development and testing data exploration**  
  Let developers use AI to explore test datasets, seed data, and local development databases stored as SQLite files during development workflows.

* **IoT and edge device data access**  
  Analyze sensor readings, event logs, and telemetry data stored in SQLite on edge devices or IoT gateways without shipping data to the cloud.

* **Embedded application reporting**  
  Access operational data from applications that use SQLite as an internal data store, such as point-of-sale systems, field service tools, and kiosk software.

* **Data science and notebook workflows**  
  Connect AI tools to SQLite databases used in Python data science environments, Jupyter notebooks, and local analysis pipelines.

* **Offline-first application analytics**  
  Analyze data from offline-capable applications that sync or store locally in SQLite files, without requiring network access or cloud sync.

## Licensing and activation

Devart MCP Server for SQLite is distributed as a free single-source MCP server.

To connect to SQLite, the server requires the corresponding [Devart ODBC Driver for SQLite](https://www.devart.com/odbc/sqlite/), which is a paid product.

A 30-day free trial is available for the Devart ODBC Driver for SQLite.

See the product page and documentation for the latest installation and activation details.

## Support

* [Documentation](https://docs.devart.com/mcp/)
* [Submit a request](https://www.devart.com/company/contactform.html)
* [Suggest a feature](https://devart.uservoice.com/)
* [Join our forum](https://support.devart.com/portal/en/community)

## Other Devart connectivity solutions

* [MCP Server Universal](https://github.com/devart-ai-connectivity/devart-mcp-server-universal)
* [ODBC Driver for SQLite](https://www.devart.com/odbc/sqlite/)
* [dotConnect ADO.NET Provider for SQLite](https://www.devart.com/dotconnect/sqlite/)
