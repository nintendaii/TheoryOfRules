using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class TasksController: MonoBehaviour
    {
        [SerializeField] private Button task1Btn;
        [SerializeField] private Button task2Btn;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private EquipmentReplacementController task1;
        [SerializeField] private InvestmentController task2;

        private void Awake()
        {
            task1Btn.onClick.AddListener(ExecuteTask1);
            task2Btn.onClick.AddListener(ExecuteTask2);
        }

        private void ExecuteTask1()
        {
            var answer = task1.Execute();
            var s = new StringBuilder();
            foreach (var it in answer)
            {
                s.Append($"Key: {it.Key} ---- Value: {it.Value.Info()} \n");
            }
            
            answerText.text = s.ToString();
        }

        private void ExecuteTask2()
        {
            var answer = task2.Execute();
            answerText.text = answer.Info();
        }
    }
}