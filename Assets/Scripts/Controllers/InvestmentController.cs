using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Models;
using UnityEngine;

namespace Controllers
{
    public class InvestmentController : MonoBehaviour
    {
        private InvestmentModel input;

        private void Start()
        {
            var outcomes = new decimal [3][];
            var incomes = new decimal [3][];
            outcomes[0] = new decimal[] { 0, 2, 4, 6, 10 };
            outcomes[1] = new decimal[] { 0, 3, 4, -1, -1 };
            outcomes[2] = new decimal[] { 1, 2, 4, -1, -1 };
            incomes[0] = new decimal[] { 0, 3, 6, 8, 12 };
            incomes[1] = new decimal[] { 0, 4, 7, -1, -1 };
            incomes[2] = new decimal[] { 0, 4, 8, -1, -1 };
            input = new InvestmentModel { n = 3, a = 10, qty = 5, outcomes = outcomes, incomes = incomes };
        }

        public InvestmentResultModel Execute()
        {
            var eq = input;
            var projects_qty = eq.qty;
            var filias_qty = eq.n;
            decimal a = eq.a;
            var outcomes = eq.outcomes;
            var incomes = eq.incomes;
            for (var i = 0; i < filias_qty; i++)
            {
                if (outcomes[i][0] == 0) continue;
                var c = outcomes[i][0];
                a -= c;
                for (var j = 0; j < projects_qty; j++)
                {
                    if (outcomes[i][j] == -1) continue;
                    outcomes[i][j] -= c;
                }
            }

            var stages = new List<decimal>[filias_qty + 1];
            var stages_incomes = new List<decimal>[filias_qty + 1];
            var project_indexes = new List<int>[filias_qty + 1];
            var lines = new List<int>[filias_qty + 1];
            var stage = new List<decimal>();
            var stage_income = new List<decimal>();
            var project_index = new List<int>();
            var line = new List<int>();
            stage.Add(0);
            line.Add(0);
            stage_income.Add(0);
            project_index.Add(0);
            stages[filias_qty] = stage;
            stages_incomes[filias_qty] = stage_income;
            project_indexes[filias_qty] = project_index;

            for (var i = filias_qty - 1; i > 0; i--)
            {
                var y = new List<decimal>();
                var y_n_plus_1 = stages[i + 1].ToArray();
                var y_n_plus_1_income = stages_incomes[i + 1].ToArray();
                var current_outcomes = outcomes[i].Where(val => val != -1).ToArray();
                var current_incomes = incomes[i].Where(val => val != -1).ToArray();
                var vectors = new decimal[y_n_plus_1.Length][];
                var local_incomes = new decimal[y_n_plus_1.Length][];

                for (var j = 0; j < y_n_plus_1.Length; j++)
                {
                    vectors[j] = new decimal[current_outcomes.Length];
                    local_incomes[j] = new decimal[current_outcomes.Length];
                    for (var l = 0; l < current_outcomes.Length; l++)
                    {
                        vectors[j][l] = current_outcomes[l] + y_n_plus_1[j];
                        local_incomes[j][l] = current_incomes[l] + y_n_plus_1_income[j];
                        y.Add(vectors[j][l]);
                    }
                }

                var uniqueList = y.Distinct().OrderBy(p => p).ToList();
                var maxIncome = new List<decimal>();
                var allProjects = new List<int>();
                var allLines = new List<int>();
                foreach (var item in uniqueList)
                {
                    var allAllProjects = new List<int>();
                    var allIncomes = new List<decimal>();
                    for (var l = 0; l < y_n_plus_1.Length; l++)
                    {
                        var indexOf = Array.IndexOf(vectors[l], item);
                        if (indexOf < 0) continue;
                        allIncomes.Add(local_incomes[l][indexOf]);
                        allAllProjects.Add(indexOf);
                    }

                    var maxElement = allIncomes.Max();
                    maxIncome.Add(maxElement);
                    var indexOfMaxElement = allIncomes.IndexOf(maxElement);
                    allLines.Add(indexOfMaxElement);
                    allProjects.Add(allAllProjects[indexOfMaxElement]);
                }

                stages[i] = uniqueList;
                lines[i] = allLines;
                stages_incomes[i] = maxIncome;
                project_indexes[i] = allProjects;
            }

            var y_n_plus_1_last = stages[1].ToArray();
            var y_n_plus_1_last_incomes = stages_incomes[1].ToArray();

            var current_outcomes_last = outcomes[0];
            var current_incomes_last = incomes[0];
            var lastMax = new List<decimal>();
            var lastEdges = new List<int>();
            foreach (var edge in y_n_plus_1_last)
            {
                var current_n = Array.IndexOf(y_n_plus_1_last, edge);
                var no = a - edge;
                var current_project_index = -1;
                for (var i = current_outcomes_last.Length - 1; i >= 0; i--)
                    if (current_outcomes_last[i] <= no)
                    {
                        current_project_index = i;
                        break;
                    }

                if (current_project_index < 0) continue;
                var localIncomeLast = current_incomes_last[current_project_index] + y_n_plus_1_last_incomes[current_n];
                lastMax.Add(localIncomeLast);
                lastEdges.Add(current_project_index);
            }

            var maxElementLast = lastMax.Max();
            var lastMaxIndex = lastMax.IndexOf(maxElementLast);
            var lastMaximum = new List<decimal>();
            lastMaximum.Add(maxElementLast);
            var lastPath = new List<int>();
            lastPath.Add(lastEdges[lastMaxIndex]);
            var lastStage = new List<decimal>();
            lastStage.Add(a);
            var lastLine = new List<int>();
            lastLine.Add(lastMaxIndex);
            stages[0] = lastStage;
            lines[0] = lastLine;
            stages_incomes[0] = lastMaximum;
            project_indexes[0] = lastPath;

            var answer_projects = new List<int>();
            var answer_outcomes = new List<decimal>();
            answer_projects.Add(lastEdges[lastMaxIndex]);
            answer_outcomes.Add(outcomes[0][lastEdges[lastMaxIndex] - 1]);
            for (var i = 1; i < filias_qty; i++)
            {
                var tempMaxIncome = stages_incomes[i].Max();
                var tempMaxIndex = stages_incomes[i].IndexOf(tempMaxIncome);
                answer_projects.Add(project_indexes[i][tempMaxIndex] + 1);
                answer_outcomes.Add(outcomes[i][project_indexes[i][tempMaxIndex]]);
            }

            var answer = new InvestmentResultModel
            {
                answer_outcomes = answer_outcomes,
                answer_projects = answer_projects,
                maxIncome = maxElementLast
            };
            return answer;
        }
    }
}