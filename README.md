
# Magic Node for ASP.NET Core

[![Build status](https://travis-ci.org/polterguy/magic.node.svg?master)](https://travis-ci.org/polterguy/magic.node)

Magic Node is a simple name/value/children graph object, in addition to a _"Hyperlambda"_ parser, allowing you to
create a textual string representations of graph objects easily transformed to the relational graph node syntax,
and vice versa. This allows you to easily declaratively create syntax trees using a format similar to YAML, for 
then to access every individual node, its value, name and children, from your C# or CLR code.

It is perfect for creating a highly humanly readable relational configuration format, or smaller DSL engines,
especially when combined with [Magic Signals](https://github.com/polterguy/magic.signals). Below is a small
example of Hyperlambda to give you an idea of how it looks like.

```
/*
 * This is a multiline comment, ignored by the parser.
 * Below is a single node, with a value of 'bar' and a name of 'foo'.
 */
foo:bar

// This is a single line comment, below is another node with an integer value.
foo:int:5

/*
 * Node with two children.
 * Notice, the value is optional, and childrne are declared
 * by adding 3 spaces in front of the child's name.
 */
foo
   child1:its-value
   child2:its-value
```
