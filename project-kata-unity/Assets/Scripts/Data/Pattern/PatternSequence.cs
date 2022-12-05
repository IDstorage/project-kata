using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Restrict only editor?
//[CreateAssetMenu(menuName = "New Pattern Sequence")]
public class PatternSequence : ScriptableObject
{
    [System.Serializable]
    public class Node
    {
        public float delay;
        public Pattern pattern;

        public Node next;

        public bool isBranch = false;
    }

    [System.Serializable]
    public class Branch : Node
    {
        public Node alternative;

        public PatternBranch condition;
    }


    public Node sequence;

    public async Task Execute(Actor target, CombatComponent combat)
    {
        var current = sequence;

        while (current != null)
        {
            var branch = current as Branch;
            if (branch != null)
            {
                current = branch.condition.UseAlternative(target, combat) ? branch.alternative : current;
            }

            await Task.Delay((int)(current.delay * 1000));
            await current.pattern.Execute(target, combat);

            current = current.next;
        }
    }

    public static void Add(Node head, float delay, Pattern pattern, Node next = null) => Add(head, new Node() { delay = delay, pattern = pattern, next = next });
    public static void Add(Node head, Node node)
    {
        node.next = head.next;
        head.next = node;
    }

    public void Replace(Node node)
    {
        node.next = sequence;
        sequence = node;
    }
}