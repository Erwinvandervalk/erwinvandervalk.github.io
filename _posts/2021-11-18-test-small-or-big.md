---
layout: post
title:  "Testing Small or Testing Big"
date:   2021-11-15 09:47:57 +0100
categories: unit-testing, practices
---

A challenge I find when designing my testing strategy is at what level should
you test? Do you **test each class individually** or do you group classes together
into components and **test the api boundaries** of these components? The challenge
here is of course, the bigger the scope of your tests, the more meaningful (and
I would argue valuable) they become, but also the more difficult certain edge
cases are to test. 

This seems to be a bit of a dichotomy. Test bigger or test smaller?

The thing is, **don't pick at all!** Use the best style of tests but deviate
from it for cases where it doesn't. Be pragmatic!

I usually build LOB applications, which means that I need to pass lots of data
around. Ingesting data in HTTP based or message based api's, processing,
validating and storing the data. Then producing forms of this data again, either
as published events or on demand (http based queries). 

Usually, I end up with several types of classes in my system:
1. **Functional classes.** These may include contracts (mapping to api's, database
   schemas, event type definitions, etc) but also the classes that operate on
   them to ingest, manipulate, validate and store the data in the database. 
2. **Supporting or technical classes.** These can be helpers to make programming
   easier, or a narrow technical solution (logging, caching, authentication,
   etc..). 

### Focussing on data flow for functional tests

For the functional classes, I try to focus on data flow through the system. That
means, modeling the actions users take and to observe the side effects. 

If you're using CQRS, a great place to do more 'functional' style testing is at
the command / query level. A command / query is a great way to express intent
and observe side effects. Writing these tests using a BDD style pattern
(Given-When-Then) also seems to be a great fit, but with a focus on observable
outcomes. 

*(Given system in state X) When the user does X (ie: save some data), the system
does Y (store the data), which you can observe by (result of action / performing
queries / observing messages).*

Ideally, I prepare the current state of my system using commands and I try to
observe the outcome using queries. The reasoning is two fold: 
1. I can more easily **change the inner implementation** without having to change my
   tests. 
2. I can create **re-usable testing components** to create the commands. This means
   that as my commands evolve over time, my tests naturally evolve with them,
   resulting in more stable test suites. 

So ideally, I will not observe that a row was added to a database (which couples
your test to your specific database implementation), but by observing the thing
the user can actually see (executing queries). 

> When building event sourced applications I usually ALSO write most of my
> business logic tests using the form: Given(Events), When (command),
> Then(Events). Combined with other test that listen for evens (Projections,
> reactors, etc.). The combination of both API based tests and more business
> logic tests absolutely kicks ass!

Now, some scenario's are bigger than a single command / query, and those are
usually a good place to do at the highest (api) level. 

### Testing Value objects 

Value objects (in domain driven design) usually provide a specific set of
functionality.

Some are so simple (just a wrapper around a value) that they don't need any form
of tests. By all means, don't write tests for them then. They will be tested as
part of the regular flow. 

> I usually don't test constructors, properties, etc in isolation. An issue here
> will quickly be found as part of my more functional business flows. Tests that
> focus on constructors or individual properties are usually very mechanical and
> don't provide any real value. 

However, sometimes you have value objects that do provide specific
functionality. If you have specialized **formatting and parsing** logic,
specialized **equality operations** or specialized **arithmetic** operations,
then writing tests for them makes a lot of sense. 

But if you're just relying on the c# record equality, parse using .net framework
primitives (Guid.Parse, Datetime.Parse, etc), or no specialized aritmetic, what
benefits do your tests bring? You're probably just testing that the .Net
framework still works. 


### Isolate technical classes from functional classes

There are a lot of classes that don't really provide much functional benefit but
do provide a good technical service. 

Sometimes, these are helpers, that make repetitive code more maintainable, or
they provide important non-functional benefits, such as authentication, caching,
logging, etc. 

It can be very useful to write good (old) unit tests focussing on one (or a
small set) of classes for these technical supporting classes. 

In the functional test cases, I do my best NOT to have to mock out these classes
but just include them in my testing flow. 

> In fact, including techncal classes in your functional test cases can
> interesting scenario's. IE: If you have caching enabled, can the user observe
> the changes you've just made or do you need some form of cache busting or
> checks for eventual consistency? 

### But, but, but.. I have some cases that are not easily tested with the examples above. 

Great, we all do. If you have certain edge cases that are both **imporant** to
test but also **difficult** to test at api / component levels, then these are a
great candidate to test in isolation. Ideally, I'd write my classes to minimize
the need to do any form of mocking, but sometimes you just don't have a choice.
In that case, go for it.

> Not all edge cases are worth testing. Consider if the cost of writing /
> maintaining the test outweighs the benefits. If it does, great, do it! If it
> doesn't, just skip it. 

# Conclusion

This heuristic has helped me a lot to write tests that are both maintainable, 
readable and very stable. I hope they help you as well. 