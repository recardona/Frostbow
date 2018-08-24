﻿using System;
using System.Collections.Generic;
using System.IO;

using BoltFreezer.Interfaces;
using BoltFreezer.PlanTools;

namespace BoltFreezer.FileIO
{
    public static class Writer
    {
        // Given a state, creates a problem PDDL file.
        public static void ProblemToPDDL(string file, Domain domain, Problem problem, List<IPredicate> state)
        {
            using (StreamWriter writer = new StreamWriter(file, false))
            {
                writer.WriteLine("(define (problem rob)");
                writer.WriteLine("(:domain " + domain.Name + ")");
                writer.Write("(:objects");
                if (!problem.Objects[0].SubType.Equals(""))
                    foreach (string type in problem.TypeList.Keys)
                    {
                        List<IObject> objects = problem.TypeList[type] as List<IObject>;
                        for (int i = 0; i < objects.Count; i++)
                        {
                            writer.Write(" " + objects[i].Name);
                            if (i == objects.Count - 1)
                                writer.WriteLine(" - " + type);
                        }
                    }
                else
                    foreach (IObject obj in problem.Objects)
                        writer.Write(" " + obj.Name);
                writer.WriteLine(")");
                writer.Write("(:init");
                foreach (IPredicate pred in state)
                    writer.WriteLine(" " + pred);
                foreach (IIntention intent in problem.Intentions)
                    writer.WriteLine(" (intends " + intent.Character + " " + intent.Predicate + ")");
                writer.WriteLine(")");
                if (problem.Goal.Count > 1)
                    writer.Write("(:goal (AND");
                else
                    writer.Write("(:goal");
                foreach (IPredicate pred in problem.Goal)
                    writer.WriteLine(" " + pred);
                if (problem.Goal.Count > 1)
                    writer.Write(")))");
                else
                    writer.Write("))");
            }
        }

        // Creates a domain PDDL file.
        public static void DomainToPDDL(string file, Domain domain)
        {
            using (StreamWriter writer = new StreamWriter(file, false))
            {
                writer.WriteLine("(define");
                writer.WriteLine("\t(domain " + domain.Name + ")");
                writer.WriteLine("\t(:requirements :adl :typing :universal-preconditions)");
                writer.WriteLine("\t(:types ");
                foreach (string type in domain.ObjectTypes)
                {
                    writer.Write("\t\t");
                    foreach (string subtype in domain.GetSubTypesOf(type))
                        writer.Write(subtype + " ");
                    writer.WriteLine("- " + type);
                }
                writer.WriteLine("\t)");
                writer.WriteLine("\t(:constants )");
                writer.WriteLine("\t(:predicates");
                foreach (Predicate pred in domain.Predicates)
                {
                    writer.Write("\t\t(" + pred.Name);
                    foreach (Term term in pred.Terms)
                    {
                        writer.Write(" " + term.Variable);
                        if (term.Type != "") writer.Write(" - " + term.Type);
                    }
                    writer.WriteLine(")");
                }
                writer.WriteLine("\t)");
                //writer.WriteLine(domain.staticStart);

                foreach (Operator action in domain.Operators)
                {
                    writer.WriteLine(Environment.NewLine + "\t(:action " + action.Name);
                    writer.Write("\t\t:parameters (");
                    foreach (Term term in action.Terms)
                    {
                        writer.Write(term.Variable + " ");
                        if (!term.Type.Equals(""))
                            writer.Write("- " + term.Type + " ");
                    }
                    writer.WriteLine(")");
                    writer.WriteLine("\t\t:precondition");
                    if (action.Preconditions.Count > 1)
                        writer.WriteLine("\t\t\t(and");
                    foreach (Predicate precon in action.Preconditions)
                        writer.WriteLine("\t\t\t\t" + precon.ToString());
                    if (action.Preconditions.Count > 1)
                        writer.WriteLine("\t\t\t)");
                    writer.WriteLine("\t\t:effect");
                    if (action.Effects.Count + action.Conditionals.Count > 1)
                        writer.WriteLine("\t\t\t(and");
                    foreach (Predicate effect in action.Effects)
                        writer.WriteLine("\t\t\t\t" + effect.ToString());
                    foreach (Axiom cond in action.Conditionals)
                    {
                        if (cond.Arity > 0)
                        {
                            writer.Write("\t\t\t\t(forall (");
                            foreach (ITerm term in cond.Terms)
                                writer.Write(term + " ");
                            writer.WriteLine(")");
                        }
                        writer.WriteLine("\t\t\t\t(when");
                        if (cond.Preconditions.Count > 1)
                            writer.WriteLine("\t\t\t\t(and");
                        foreach (Predicate precon in cond.Preconditions)
                            writer.WriteLine("\t\t\t\t\t" + precon.ToString());
                        if (cond.Preconditions.Count > 1)
                            writer.WriteLine("\t\t\t\t)");
                        if (cond.Effects.Count > 1)
                            writer.WriteLine("\t\t\t\t(and");
                        foreach (Predicate effect in cond.Effects)
                            writer.WriteLine("\t\t\t\t\t" + effect.ToString());
                        if (cond.Effects.Count > 1)
                            writer.WriteLine("\t\t\t\t)");
                        writer.WriteLine("\t\t\t\t)");
                        if (cond.Arity > 0)
                            writer.WriteLine("\t\t\t\t)");
                    }
                    if (action.Effects.Count + action.Conditionals.Count > 1)
                        writer.WriteLine("\t\t\t)");
                    writer.WriteLine("\t)");
                }

                writer.WriteLine(")");
            }
        }

        // Create a summary of the tree generated.
        public static void Summary(string directory, List<Utilities.Tuple<String, String>> content)
        {
            string file = directory + "index.html";

            using (StreamWriter writer = new StreamWriter(file, false))
            {
                writer.WriteLine("<html>");
                writer.WriteLine("<body>");
                foreach (Utilities.Tuple<String, String> tuple in content)
                {
                    writer.WriteLine("<b>" + tuple.First + "</b><br />");
                    writer.WriteLine(tuple.Second + "<br /><br />");
                }
                writer.WriteLine("<b>Root State</b><br />");
                writer.WriteLine("<a href='node-0.html'>Root</a><br />");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        // Generates a CSV file of test statistics.
        public static void ToCSV(string directory, List<List<Utilities.Tuple<String, String>>> summaries)
        {
            string file = directory + "summary.csv";

            using (StreamWriter writer = new StreamWriter(file, false))
            {
                List<Utilities.Tuple<String, String>> firstSummary = summaries[0];
                Utilities.Tuple<String, String> lastTuple = firstSummary[firstSummary.Count - 1];
                firstSummary.RemoveAt(firstSummary.Count - 1);
                foreach (Utilities.Tuple<String, String> tuple in firstSummary)
                    writer.Write(tuple.First + ",");
                writer.WriteLine(lastTuple.First);
                foreach (Utilities.Tuple<String, String> tuple in firstSummary)
                    writer.Write(tuple.Second + ",");
                writer.WriteLine(lastTuple.Second);

                summaries.RemoveAt(0);

                foreach (List<Utilities.Tuple<String, String>> summary in summaries)
                {
                    lastTuple = summary[summary.Count - 1];
                    summary.RemoveAt(summary.Count - 1);
                    foreach (Utilities.Tuple<String, String> tuple in summary)
                        writer.Write(tuple.Second + ",");
                    writer.WriteLine(lastTuple.Second);
                    summary.Add(lastTuple);
                }
            }
        }
    }
}
