using System.Collections.Generic;
using DefaultNamespace;

namespace Models
{
    public class InvestmentModel
    {
        public int n { get; set; }
        public int a { get; set; }
        public int qty { get; set; }
        public decimal[][] incomes { get; set; }
        public decimal[][] outcomes { get; set; }
        
    }

    public class InvestmentResultModel
    {
        public List<int> answer_projects { get; set; }
        public List<decimal> answer_outcomes { get; set; }
        public decimal maxIncome { get; set; }

        public string Info()
        {
            return $"Answer projects: {answer_projects.ToListString()} \n Outcomes: {answer_outcomes.ToListString()} \n Maximum Income: {maxIncome}";
        }
    }
}