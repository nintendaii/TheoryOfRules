using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace;
using Models;
using UnityEngine;

namespace Controllers
{
    public class EquipmentReplacementController : MonoBehaviour
    {
        private EquipmentModel input;
        private Dictionary<int, NodeModel> output = new Dictionary<int, NodeModel>();

        private void Start()
        {
            decimal[] rt = { 20.0M, 19.0M, 18.5M, 17.2M, 15.5M, 14M, 12.2M };
            decimal[] ut = { 0.2M, 0.6M, 1.2M, 1.5M, 1.7M, 1.8M, 2.2M };
            decimal[] st = { 0M, 80M, 60M, 50M, 30M, 10M, 5M };
            Initialize(5, 100, 2, rt, ut, st);
            foreach (var it in output)
            {
                Debug.Log($"Key: {it.Key} ---- Value: {it.Value.Info()}");
            }
        }


        private void Initialize(int nIn, int pIn, int tYears, decimal[] rtDec, decimal[] utDec, decimal[] stDec)
        {
            input = new EquipmentModel { n = nIn, p = pIn, t_years = tYears, rt = rtDec, ut = utDec, st = stDec };
        }

        public Dictionary<int, NodeModel> Execute()
        {
            var eq = input;
            var n = eq.n;
            decimal p = eq.p;
            var t_years = eq.t_years;
            var qty = n + t_years;
            var t = new int[qty];
            var rt = eq.rt;
            var ut = eq.ut;
            var st = eq.st;
            //double[] rt = { 20.0, 19.0, 18.5, 17.2, 15.5, 14, 12.2 };
            //double[] ut = { 0.2, 0.6, 1.2, 1.5, 1.7, 1.8, 2.2 };
            //double[] st = { 0, 80, 60, 50, 30, 10, 5 };
            var steps = new int[n][];
            var steps_old = new decimal[n][];
            var steps_new = new decimal[n][];
            var f_tau = new decimal[n][];
            var first_step = new int[1];
            first_step[0] = t_years;
            steps[0] = first_step;
            int array_size;
            for (var i = 1; i < n; i++)
            {
                array_size = steps[i - 1].Length;
                var step = new int[array_size + 1];
                step[0] = 1;
                for (var j = 1; j <= array_size; j++) step[j] = steps[i - 1][j - 1] + 1;
                steps[i] = step;
            }
            Debug.Log("2d array steps: "+steps.ToMatrixString());
            var last_step = steps[n - 1];
            var last_step_size = last_step.Length;
            var last_step_old = new decimal[last_step_size];
            var last_step_new = new decimal[last_step_size];
            var last_step_f = new decimal[last_step_size];
            int local_t;
            for (var i = 0; i < last_step_size; i++)
            {
                local_t = last_step[i];

                try
                {
                    last_step_old[i] = rt[local_t] - ut[local_t] + st[local_t + 1];
                }
                catch (IndexOutOfRangeException exception)
                {
                    Debug.Log("eececcecececc");
                    exception.ToString();
                    last_step_old[i] = -1;
                }

                Debug.Log($"{last_step_size} {last_step.Length} {st.Length}");
                last_step_new[i] = rt[0] - ut[0] + st[local_t] - p + st[1]; //exception here

                last_step_f[i] = Math.Max(last_step_new[i], last_step_old[i]);
            }

            steps_old[n - 1] = last_step_old;
            steps_new[n - 1] = last_step_new;
            f_tau[n - 1] = last_step_f;

            for (var i = n - 2; i >= 0; i--)
            {
                array_size = steps[i].Length;
                var step_old = new decimal[array_size];
                var step_new = new decimal[array_size];
                var step_f = new decimal[array_size];

                for (var j = 0; j < array_size; j++)
                {
                    local_t = steps[i][j];
                    step_old[j] = rt[local_t] - ut[local_t] + f_tau[i + 1][j + 1];
                    step_new[j] = rt[0] - ut[0] + st[local_t] - p + f_tau[i + 1][0];
                    //  step_new[j] = Math.Round(step_new[j], 1);
                    step_f[j] = Math.Max(step_new[j], step_old[j]);
                }

                steps_new[i] = step_new;
                steps_old[i] = step_old;
                f_tau[i] = step_f;
            }

            var answers = new Dictionary<int, NodeModel>();
            var id = 1;
            var age = t_years;
            var current_step = 0;
            var first = new NodeModel
            {
                parent_id = null
            };
            if (steps_new[0][0] > steps_old[0][0])
            {
                first.age = 1;
                first.answer = true;
                first.id = id;
                first.step = current_step;
                first.name = "Замінити";
            }
            else
            {
                first.age = t_years + 1;
                first.answer = false;
                first.id = id;
                first.step = current_step;
                first.name = "Залишити";
            }

            answers.Add(id, first);
            int previous_step;
            for (var i = 1; i < n; i++)
            {
                previous_step = i - 1;
                var keys = answers.Where(item => item.Value.step.Equals(previous_step))
                    .Select(item => item.Key).ToArray();
                foreach (var answer_key in keys)
                {
                    id++;
                    var answer = answers[answer_key];
                    var current_age = answer.age;
                    var current_age_index = Array.IndexOf(steps[i], current_age);
                    var node_item = new NodeModel();
                    node_item.parent_id = answer.id;
                    if (steps_new[i][current_age_index] > steps_old[i][current_age_index] ||
                        steps_old[i][current_age_index] < 0)
                    {
                        node_item.age = 1;
                        node_item.answer = true;
                        node_item.id = id;
                        node_item.step = i;
                        node_item.name = "Замінити";
                    }
                    else
                    {
                        node_item.age = current_age + 1;
                        node_item.answer = false;
                        node_item.id = id;
                        node_item.step = i;
                        node_item.name = "Залишити";
                    }

                    if (steps_new[i][current_age_index] == steps_old[i][current_age_index])
                    {
                        node_item.age = 1;
                        node_item.answer = true;
                        node_item.id = id;
                        node_item.step = i;
                        node_item.name = "Замінити";
                        var second_node_item = new NodeModel();
                        second_node_item.parent_id = answer.id;
                        second_node_item.age = current_age + 1;
                        second_node_item.answer = false;
                        second_node_item.id = id;
                        second_node_item.step = i;
                        second_node_item.name = "Залишити";
                        answers.Add(id, second_node_item);
                        answers[answer_key].children.Add(id, second_node_item);
                        id++;
                    }

                    answers.Add(id, node_item);
                    answers[answer_key].children.Add(id, node_item);
                }
            }

            output = answers;
            return output;
        }
        
        
    }
    
}