---
layout: post
title:  "Characteristics of well written tests"
date:   2021-11-15 09:47:57 +0100
categories: unit-testing, practices
---

We don't write tests 'just for fun', but because we believe that it brings certain important benefits. Without (enough of) these benefits, it's 
just overhead and useless ceremony. So the question is, what are the characteristics that make unit testing worth while? And what issues should we avoid?

When speaking to various people and reviewing a LOT of code, i'm finding that many people have different thoughts and idea's about this. I'm quite passionate about automated testing, so I figured I'd jot down my own thoughts here.  

> I started writing a completely different blog post.. Then from there, this post started to form.. then it became too big. WAY to big. So I cut it down so I can at least get something out. Hopefully, I'll be able to post the others soon as well. 

In this post, i'm talking about automated tests, but they apply equally well to unit tests, integration tests, UI tests, etc..

# Characteristics of well written tests. 

A good test helps you by:

1. Bringing you **increased understanding**. It helps you to write the code, or when reading the tests to understand what the code does. 
2. **Protects** you from mistakes during writing / changing / refactoring and bring you **confidence** in the correctness of the code. 
3. Being **resilient** against internal changes. It should not fail if you refactor the insides of your test subjects. 
4. Be **easily changed** when changes to api signatures occur.
5. Making software development **faster**, not slowing you down. 
   

In my mind, if your tests do not have all of these characteristics, that's a smell. But if they are missing more than one, they might be causing more
harm than good. 

<br>
Looking at the inverse of these properties can also be interesting. A test should **not**: 
1. **Slow me down** when writing or maintaining code. If there's too much overhead in writing / maintaining / running the tests, they don't bring me any value. How you factor your code is important as well as at which level you start your tests.  
2. **Confuse me**: If you can't understand what a test does, how it does things or why it does things, it's usually not valuable at all. A big reason for this can be the use of mocking frameworks (more) about that in a follow up post. 
3. **undermine my trust**. If you can't trust your tests, they are not very valuable. This can happen when your tests don't catch mistakes made. Probably the coverage or assertions are not good enough to build trust. Another reason to undermine your trust is if they fail without good reason. The tests are too brittle then and, because I have to go in and modify my tests as well, i'm eroding the value that they bring. Flakey tests are the worst, when the tests occasionally fail, usually due to improper test isolation. *Looking at you, static mutable state!*

## How to achieve these characteristics? 

Achieving the characteristics above is not as simple. So, how can you achieve these characteristics?
Each test should:
1. have a clear **purpose**. Sometimes this might mean test a single thing (even if the single thing is a complex business scenario). The name of the test is important, but so is the way you write it. 
2. actually protect you from **real issues**. I've seen too many tests that don't actually protect you from anything. I'll probably write another blog post later about things not worth testing. 
3. **Draw attention** to the things that are important in your test, while hiding the things which are not. Think deep and hard about what you want your test to express. What are the things that are different in this test from the other tests? This ties in again to it's purpose. Hiding the unimportant ceremony in helpers (such as data builders, fixtures, helper methods, etc) has the benefit of both **focussing attention** on what IS important but also allowing you to **reuse** the logic. 
4. Avoid **static mutable state** like the plague. Static mutable state is the thing that makes your test run fine when running them one at a time, but fail when you run them together, or on the build server. 
5. Be written at the **appropriate level** of technical complexity. Business oriented components have different requirements than more technical components. 
6. Clearly express the **reason for failures**. Tests will eventually fail (otherwise, what's their purpose?). When a test fails, should be quickly clear why they fail, ideally without the need for debugging. I've often found printing log messages to the test output window is a very powerful method for this.. 

In a future post, I'll try to write more about how I typically structure my tests and what things to avoid. 

# Conclusion

With these characteristics, I've often found my tests to be easy to understand (at least to me), easy to maintain. Hopefully, you find it useful. 




