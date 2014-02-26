Umbraco.Backend.Restriction
=========

This HttpModule, was made to restrict the access to the ADMIN part of the CMS, and other sensibles resources that you don't want share with Front End users.

If you are over Umbraco v6 using the mvc features (SurfaceControllers, API, etc) the default routes i.e: ```http://[ROOT_SITE]/umbraco/surface/MyController/SomeAction``` are excluded from this restriction.

Installation
--

* Add the Umbraco.Backend.Restriction.dll to umbraco bin directory.
* Add ``BackendRestriction.json`` to umbraco config folder **~/Config**
* Register the HttpModule on your **web.config**; to keep compatibility with new/old iis the module is registered twice:
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
    * under ``<system.web>`` element:
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

The module works with a set of rules/parametters specified in the config file ``BackendRestriction.json``, following the next structure.

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
  ]
}
```
* **RegexForbiddenRoutes**:
    Array of regex that will be used to detect, safe pahts in the example it will match agains any resource under /umbraco/ path, having ``.aspx`` ``.asmx`` ``.ashx`` estensions. With this filter you will refuse accesses to the common umbraco admin resources included one of the most important the **login page** (~/umbraco/login.aspx). You can have more than one pattern, but at **least one** must be defined.
* **SafeHosts**:
    Array of safe hosts that have granted acces to forbbiden routes, if the port is anything buy ``80`` you need to append it after the host.
* **SafeIps**:
    Array of safe IPs that have granted acces to forbbiden routes.

*You can have both *SafeIps* and *SafeHosts* on the same config, but only one is required.*

Use, debug, modify the source code.
--
The solution is on 2012 VS, all dependencies are linked to NuGet. So you need to re download them from NuGet manager.

Since this is a feature related to your **system security**; I strongly recommend, make use of test solution provided test cases are created with **Nunit** in order to run these the WebTestApp are needed; both included in the source code (you will find a set of basic test as examples), add your own test depending on what you want allow/deny and once you have all scenarios covered, test it again under an umbraco instance.

External tools
--

- Nunit: http://www.nunit.org/
- Json.NET: http://james.newtonking.com/json

Licence
--

MIT