# QuickBooksASPNetCore2OpenIDConnect
Example of how to access QuickBooks Online via ASP.NET Core 2 &amp; OpenID Connect Provider

This is a minimal sample that displays customers from QuickBooks Online.  It is not intended to be used as a production solution.

## Getting Started

These instructions will get you a project up and running that demonstrates everything you need to build an ASP.NET Core 2 web application that will use the single sign on capabilities of QuickBooks online usinge the Microsoft supplied OpenId Connect authentication provider.  You can also clone this repro and run the application after making a couple of minor changes to specify your QuickBooks App Client ID and Client Secret

### Prerequisites

You'll need to create a QuickBooks app and copy the Client ID and Client Secret. [Intuit Developer Getting Started](https://developer.intuit.com/getstarted)

[.NET Core](https://www.microsoft.com/net/core#windowscmd)

### Configuring ASP.NET Core Web App

A [Step By Step Guide](https://github.com/ehalsey-quickbooks/QuickBooksASPNetCore2OpenIDConnect/wiki/Step-By-Step-Guide) with screen shots

## Built With

* [QuickBooks Accounting API](https://developer.intuit.com/getstarted) - For single sign on, authorization and query accounting data
* [.NET Core](https://www.microsoft.com/net/core#windowscmd) - Hosts the web application
* [json2csharp](http://json2csharp.com/) - Used to generate POCO classes from Json samples.

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Eric Halsey** - *Initial work* - Always looking for challenging projects

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Intuit QuickBooks Developer Group 
** [Nimisha Shrivastava](https://help.developer.intuit.com/s/profile/005G0000002k5Q9IAI)

