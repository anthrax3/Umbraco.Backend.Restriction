Umbraco Backend Restriction
=========

This HttpModule was made to restrict the access to the ADMIN part of the CMS and other sensibles resources that you don't want to share with Front End users.

If you are over Umbraco v6 using the mvc features (SurfaceControllers, API, etc) the default routes i.e: ```http://[ROOT_SITE]/umbraco/surface/MyController/SomeAction``` are excluded from this restriction.

Installation
--

* Add the Umbraco.Backend.Restriction.dll to umbraco bin directory.
* Add ``BackendRestriction.json`` to umbraco config folder **~/Config**
* Register the HttpModule on your **web.config**; to keep compatibility with new/old iis this module is registered twice:
    * under ``<system.webServer>`` element:
    ```
    <system.webServer>
        <modules runAllManagedModulesForAllRequests="true">
            <add type="Umbraco.Backend.Restriction.Backend, Umbraco.Backend.Restriction" name="Backend" />
            ...
        </modules>
        ...
    </system.webServer>
    ```
    * and under ``<system.web>`` element:
    ```
    <system.web>
        <httpModules>
            <add type="Umbraco.Backend.Restriction.Backend, Umbraco.Backend.Restriction" name="Backend" />
            ...
        </httpModules>
        ...
    </system.web>
    ```

Settings
--

The module works with a set of rules/parametters specified in the config file ``BackendRestriction.json``, with the following structure.

```
{
  "RegexForbiddenRoutes": [
    {
      "pattern": "^\\/umbraco\\/.*(\\.aspx|\\.asmx|\\.ashx)$",
      "options": 1,/* case insensitive */
      "matchTimeout": -10000 /* default */
    }
  ],
  "SafeHosts": [
    "allowed.local:18077",
	"ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt:18077"
  ],
  "SafeIps": [
    "127.0.0.1" , "198.168.1.110"
  ],
  "BasicAuthUsers": {
    "user": "pass",
    "rambo": "john"
  },
  "UseBasicAuth" : false
}
```
* **RegexForbiddenRoutes**:
    Array of regex that will be used to detect safe paths. In the example it will match against any resource under /umbraco/ with the following extensions ``.aspx`` ``.asmx`` ``.ashx``. With this filter you will deny access to the common umbraco admin resources included the **login page** (~/umbraco/login.aspx). You can have more than one pattern, but at **least one** must be defined.
* **SafeHosts**:
    Array of safe hosts that have allowed access to forbbiden routes, if the port is anything but ``80`` you need to add it after the host.
* **SafeIps**:
    Array of safe IPs that have granted access to forbbiden routes.
* **UseBasicAuth**:
    Enables basic auth before the host/ip validation.
* **BasicAuthUsers**:
    Array of users to check when basic auth is *true*.

* *You can have both: *SafeIps* and *SafeHosts* on the same config, but only one is required.*
* *Basic Auth feature is not recomended for production eviroment, is just another way to restrict the access.*

Use, debug, modify the source code.
--
The solution is on 2012 VS, all dependencies are linked to NuGet. So you need to download them from NuGet.

External tools
--

- Nunit: http://www.nunit.org/
- Json.NET: http://james.newtonking.com/json

Licence
--

MIT
