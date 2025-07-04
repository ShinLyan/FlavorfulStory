using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.Core
{
    /// <summary> Конъюнкция — логическая операция И. Истинна, если все условия истинны. </summary>
    /// <remarks> Пример сложного условия: (Завершен квест И есть предмет) ИЛИ (НЕ заблокирован проход) </remarks>
    [Serializable]
    public class Condition
    {
        /// <summary> Массив дизъюнкций, которые объединяются по И. </summary>
        [SerializeField] private Disjunction[] _and;

        /// <summary> Проверяет выполнение всех дизъюнкций (конъюнкция). </summary>
        /// <param name="evaluators"> Список объектов, реализующих интерфейс IPredicateEvaluator. </param>
        /// <returns> True, если все дизъюнкции истинны; иначе — false. </returns>
        public bool Check(IEnumerable<IPredicateEvaluator> evaluators) =>
            _and.All(disjunction => disjunction.Check(evaluators));

        /// <summary> Дизъюнкция — логическая операция ИЛИ. Истинна, если хотя бы одно условие истинно. </summary>
        [Serializable]
        public class Disjunction
        {
            /// <summary> Массив предикатов, объединяемых по ИЛИ. </summary>
            [SerializeField] private Predicate[] _or;

            /// <summary> Проверяет выполнение хотя бы одного предиката (дизъюнкция). </summary>
            /// <param name="evaluators"> Список объектов, реализующих интерфейс IPredicateEvaluator. </param>
            /// <returns> True, если хотя бы один предикат истинный; иначе — false. </returns>
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators) =>
                _or.Any(predicate => predicate.Check(evaluators));
        }

        /// <summary> Отдельное условие (предикат) с параметрами и возможным отрицанием. </summary>
        [Serializable]
        public class Predicate
        {
            /// <summary> Имя предиката для проверки. </summary>
            [SerializeField] private string _predicate;

            /// <summary> Параметры для передачи в предикат. </summary>
            [SerializeField] private string[] _parameters;

            /// <summary> Флаг для применения отрицания (НЕ) к результату проверки. </summary>
            [SerializeField] private bool _negate;

            /// <summary> Проверяет условие с учетом отрицания. </summary>
            /// <param name="evaluators"> Список объектов, реализующих интерфейс IPredicateEvaluator. </param>
            /// <returns> True, если условие выполняется; иначе — false. </returns>
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators) => evaluators
                .Select(evaluator => evaluator.Evaluate(_predicate, _parameters))
                .All(result => result != _negate);
        }
    }
}