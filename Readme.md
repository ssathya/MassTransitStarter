# Masstransit-RabbitMQ
I stumbled upon multiple hurdles when I was trying to use Masstransit as a console application. Yes, there are YouTube tutorials and StackOverflow discussions but they were for "Hello World" kind of applications. Did a bit of digging and finally did find the solution at [Masstransit's website](https://masstransit-project.com/usage/containers/msdi.html) itself that I ignored to investigate.

There are multiple projects but the ones that work/use are:

|Project Name|Remarks  |
|--|--|
|DateHandler  |Publisher|
|MTCommon|Common stuff across projects|
|BookExample| Subscriber that works|
|DateSubscriber|Subscriber that works in .Net Core API project|

There are bunch of other projects=> failed attempts.


