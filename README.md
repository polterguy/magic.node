
# Magic Node for ASP.NET Core

[![Build status](https://travis-ci.org/polterguy/magic.node.svg?master)](https://travis-ci.org/polterguy/magic.node)

Magic Node is a simple name/value/children graph object, in addition to a _"Hyperlambda"_ parser, allowing you to
create a textual string representations of graph objects easily transformed to the relational graph node syntax,
and vice versa. This allows you to easily declaratively create syntax trees using a format similar to YAML, for 
then to access every individual node, its value, name and children, from your C# or CLR code.

It is perfect for creating a highly humanly readable relational configuration format, or smaller DSL engines,
especially when combined with [Magic Signals](https://github.com/polterguy/magic.signals).
