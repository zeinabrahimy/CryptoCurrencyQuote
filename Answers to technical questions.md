1\. How long did you spend on the coding assignment? What would you add
to your solution if you had more time? If you didn't spend much time on
the coding assignment then use this as an opportunity to explain what
you would add. 

- I spent about 16 hours. ( 3 Evenings after work)
- If I had enough time I would :
	- Make a proper board for the project and split the works into short tasks to make them deliverable in sprint period, 
	- Discuss the details to make the service more robust,
	- Add a web api to expose the service methods with all test types, e.g. Integration tests.
	- Add a react ui project with focus on UX and requirements of the users.
	- Add Sub-Scenarios To Unit Tests,
	- Think and discuss security concerns based on type of the project’s usage and when it’s going to take place, 
	- Dockerize the project and create CI/CD pipeline for it to bring agility for deliver the features,
	- Define scalability strategy for project and implement it based on project resources and requirement,
	- Consider and add a trustable and fully featured Monitoring, Insights and metrics plan based on base platform policies, like IndightOps, New Relic Or Humio and Sanity platforms, so we will always have a wide range of visibility over platform to track the systems health and performance.


2\. What was the most useful feature that was added to the latest version
of your language of choice? Please include a snippet of code that shows
how you've used it.

Record types have recently been added to C\#. We had a lack in C\# which
caused developers couldn’t create Object Values in DDD and compare them.
Record types are immutable by default and consider two records equal if
their properties are all equal.

[Record Types in C\#](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9\#record-types)


3\. How would you track down a performance issue in production? Have you
ever had to do this?

- Track down a performance issue

    - Performance test: Write a performance test in an environmental Laboratory. Run and check the result. Probably many factor exist:
    - Algorithm used: read and check codes, use different counters and metrics to determine which part of the algorithm takes more time.
    - Sync and Async Structure: Parallel tasks that can run at the same time.
    - SQL performance issues:
	    - Check written query does not have any issue problems and optimize them. 
	    - The load amount on insert and update
    - Create Index
    - Using Monitoring tools to understand the bottleneck and find proper solution for it


- Experience in Performance issue
We had a problem in concurrent rounds played with many clients. 
At first we wrote a performance test for 300 clients. ( With NBomber Framework https://github.com/PragmaticFlow/NBomber). 
We checked the algorithm used and realized that because of speed update and insert on round entity, RMDBS databases aren’t useful for this scenario so we switched from SQL to MongoDB on this part of application.

4\. What was the latest technical book you have read or tech conference
you have been to? What did you learn?

I haven't read the complete technical book for a while, but I remember that last week, I was reading Domain Driven Design, and its author, Eric Evans, explained how much a common language is important. So I thought about the misundertanding in the meetings that causes a lot of bugs in the software.

But as a developer who is interested in his work, reading articles in microsoft documents and LinkedIn is obviously and one of my hobbies. Also blow books are my favorite books:

- Clean Code:
	- In Short, Leave the code better than you find it! Write simple and clean code,
- Microservices using containers
	- How to think in a way that we could be able to scale up and scale down seamlessly, 
	- How to manage to have secure boundary over sub services, How to split the services responsibility based on business logics and also achieve single responsibility in service layer, 
	- How to orchestrate huge systems with microservice architecture, Resolve Complicated Systems.



5\. What do you think about this technical assessment?

The problem was good and open so that I could be creative. It had a few
small challenges. I made two solutions, although I know there are other
solutions.

But given that there was little time and I am now an employee, if I had
more time I would implement a WebApi and provide a usable output for
others. And maybe in the future I will plan to complete it, push it in
Github as an open source project to display graphs that show crypto
currencies quote instantly and automatically. There are sites that do
this but do not offer free APIs.

6\. Please, describe yourself using JSON.
```json
{
	"firstName": "Zeinab",
	"lastName": "Rahimy",
	"jobTitle": "Software Engineer",
	"educations": [
		{
			"level": "Bachelor Degree",
			"university": "Payame Noor Tehran",
			"graduatedDate": "2011",
			"subject": "Computer Science"
		}
	],
	"experiences": [
		{
			"companyName": "Rayanmehr",
			"location": "Tehran",
			"startDate": "2019-03-25",
			"endDate": null,
			"posts": [
				{
					"title": "Senior Software Developer",
					"projects": [
						{
							"title": "eCharge",
							"responsibility": "As a backend developer, I am working on developing new features, maintain legacy codes, work on the performance of some pieces of product, and handle technical depts that existed during all past years.",
							"description": " eCharge (top-up and prepaid services for all Iranian mobile operators)"
						},
						{
							"title": "Challesh",
							"responsibility": "I created the product based on microservices architecture and DDD approach to control complexity around the problem domain. As a technical view, it has used ASP.NET Core Web API to expose each microservice and MS SQL Server and MongoDB as databases. This project has used a range of technologies, most of them based on .NET: Windsor Castle, Dapper, xUnit, Moq, and some other technologies to create microservices. Microservices use HTTP call for request/response communication between services and VerneMQ as an Events Hub for Pub/Sub communication between them. In the front-end, a SPA that is created using ReactJS as an administration portal, and for playing games,",
							"description": "Challesh is a SaaS product, and its target is creating an infrastructure for games that offers a set of abilities that any games need them."
						}
					]
				}
			]
		},
		{
			"companyName": "Chargoon",
			"location": "Tehran",
			"startDate": "2013-06-11",
			"endDate": "2019-03-25",
			"posts": [
				{
					"title": "Senior Software Developer In Automation Solution",
					"projects": [
						{
							"title": "Re-engineering Automation",
							"responsibility": "Designed, implemented and reviewed other developer codes",
							"description": "Automation is a domain with five software (official correspondence, archive document management, task management, calendar and meeting management, and a common software included all common features for use in other software) that manages official workflows in the big scale organizations."
						},
						{
							"title": "Organization Meetings",
							"responsibility": "Developed the backend of product using ASP.NET Web API, Windsor Castle as Ioc Container and Entity Framework in the repository layer",
							"description": "Organization Meetings is a software like Outlook to manage and handle meetings and calendars in big organizations using their concepts. This software manages calendars, meetings, and their members in an organization. This project used DDD to resolve complexity"
						}
					]
				},
				{
					"title": "Web Developer",
					"projects": [
						{
							"title": "Budget And Bureau",
							"responsibility": "Designed and developed features in the software and participated in analysis, design and development of Budget systems using industry standard tools",
							"description": "A financial software in the big scale organizations."
						}
					]
				}
			]
		},
		{
			"companyName": "ABC Idea",
			"location": "Tehran",
			"startDate": "2011-06-01",
			"endDate": "2013-06-11",
			"posts": [
				{
					"title": "Web Developer",
					"projects": [
						{
							"title": "Web Sites",
							"responsibility": "Implement more than 200 web sites base on asp.net and sql",
							"description": " Ads, News and Shopping sites"
						}
					]
				}
			]
		}
	],
	"certificates": [
		"MCTS",
		"MCPD"
	],
	"skills": [
		"Programming",
		"C#",
		"asp.NET",
		"WebAPI",
		"TSQL",
		"MongoDB",
		"MessageQueue",
		"EntityFramework",
		"Dapper",
		".netCore",
		"Database design",
		"Git",
		"TDD",
		"DDD",
		"Agile Methodology",
		"Problem resolution",
		"Kibana",
		"ElasticSearch"
	]
}
```
