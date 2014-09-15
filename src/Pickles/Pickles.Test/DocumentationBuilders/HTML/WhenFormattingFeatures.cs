﻿using System;
using System.Linq;
using System.Xml.Linq;
using NFluent;
using NUnit.Framework;
using Autofac;
using PicklesDoc.Pickles.DocumentationBuilders.HTML;
using PicklesDoc.Pickles.ObjectModel;

namespace PicklesDoc.Pickles.Test.DocumentationBuilders.HTML
{
  [TestFixture]
  public class WhenFormattingFeatures : BaseFixture
  {
    [Test]
    public void ThenCanFormatDescriptionAsMarkdown()
    {
      var feature = new Feature
      {
        Name = "A feature",
        Description =
            @"In order to see the description as nice HTML
As a Pickles user
I want to see the descriptions written in markdown rendered as HTML

Introduction
============

This feature should have some markdown elements that get displayed properly

Context
-------

> I really like blockquotes to describe
> to describe text

I also enjoy using lists as well, here are the reasons

- Lists are easy to read
- Lists make my life easier

I also enjoy ordering things

1. This is the first reason
2. This is the second reason"
      };

      var htmlFeatureFormatter = Container.Resolve<HtmlFeatureFormatter>();
      XElement featureElement = htmlFeatureFormatter.Format(feature);
      XElement description = featureElement.Elements().FirstOrDefault(element => element.Name.LocalName == "div");

      Assert.NotNull(description);
      Check.That(description).IsNamed("div");
      Check.That(description).IsInNamespace("http://www.w3.org/1999/xhtml");
      Check.That(description).HasAttribute("class", "description");
      Assert.AreEqual(9, description.Elements().Count());

      Check.That(description.Elements().ElementAt(0)).IsNamed("p");
      Check.That(description.Elements().ElementAt(1)).IsNamed("h1");
      Check.That(description.Elements().ElementAt(2)).IsNamed("p");
      Check.That(description.Elements().ElementAt(3)).IsNamed("h2");
      Check.That(description.Elements().ElementAt(4)).IsNamed("blockquote");
      Check.That(description.Elements().ElementAt(5)).IsNamed("p");
      Check.That(description.Elements().ElementAt(6)).IsNamed("ul");
      Check.That(description.Elements().ElementAt(7)).IsNamed("p");
      Check.That(description.Elements().ElementAt(8)).IsNamed("ol");
    }

    [Test]
    public void ThenCanFormatMarkdownTableExtensions()
    {
      var feature = new Feature
      {
        Name = "A feature",
        Description =
@"In order to see the description as nice HTML
As a Pickles user
I want to see the descriptions written in markdown rendered with tables

| Table Header 1 | Table Header 2 |
| -------------- | -------------- |
| Cell value 1   | Cell value 2   |
| Cell value 3   |                |
| Cell value 4   | Cell value 5   |
"
      };

      var htmlFeatureFormatter = Container.Resolve<HtmlFeatureFormatter>();
      XElement featureElement = htmlFeatureFormatter.Format(feature);
      XElement description = featureElement.Elements().FirstOrDefault(element => element.Name.LocalName == "div");

      Assert.NotNull(description);
      Check.That(description).IsNamed("div");
      Check.That(description).IsInNamespace("http://www.w3.org/1999/xhtml");
      Check.That(description).HasAttribute("class", "description");

      XElement table = description.Descendants().FirstOrDefault(el => el.Name.LocalName == "table");

      Assert.IsNotNull(table);

    }

    [Test]
    public void ThenCanFormatMarkdownTableExtensionsEvenIfTheyAreSomewhatMalstructured()
    {
      var feature = new Feature
      {
        Name = "A feature",
        Description =
@"In order to see the description as nice HTML
As a Pickles user
I want to see the descriptions written in markdown rendered with tables

| Table Header 1 | Table Header 2                         |
| -------------- | -------------------------------------- |
| Cell value 1   | Cell value 2                           |
| Cell value 3     Note the missing column delimiter here |
| Cell value 4   | Cell value 5                           |
"
      };

      var htmlFeatureFormatter = Container.Resolve<HtmlFeatureFormatter>();
      XElement featureElement = htmlFeatureFormatter.Format(feature);
      XElement description = featureElement.Elements().FirstOrDefault(element => element.Name.LocalName == "div");

      Assert.NotNull(description);
      Check.That(description).IsNamed("div");
      Check.That(description).IsInNamespace("http://www.w3.org/1999/xhtml");
      Check.That(description).HasAttribute("class", "description");

      XElement table = description.Descendants().FirstOrDefault(el => el.Name.LocalName == "table");

      Assert.IsNotNull(table);

    }
  }
}