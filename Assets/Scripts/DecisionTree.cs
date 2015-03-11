using UnityEngine;
using System.Collections;
using System.IO;

public class DecisionTree : MonoBehaviour {

	private const string treePath = "DecTreeFile.txt";
	// Data file expected in bin folder
	
	Node root = null;
	// Tree is empty (null) by default
	
	public Node Root
	{
		get { return root; }
		set { root = value; }
	}
	
	public DecisionTree ()
	{
		root = null;        // Redundant, but hey, better safe than sorry
	}
	
	// Don't need the WriteTree methods for the simple decision tree demo because
	// this program doesn't learn, but left them in anyway.
	
	// Opens the datafile in write/replace mode and calls the preorder traversal
	public void WriteTree ()
	{
		if (root != null) {			// Make sure tree is not empty
			string fName = treePath;
			StreamWriter outStream = new StreamWriter (fName);
			RecWriteTree (root, outStream);
			outStream.Close ();
		}
		else
			Debug.Log("I know nothing, so I have nothing to remember.");
	}
	
	// Standard pre-order traversal to write the tree to a text file, one node per line
	void RecWriteTree (Node tree, StreamWriter outStream)
	{
		if (tree.NoPtr == null) {
			outStream.Write ("L");
			outStream.WriteLine (tree.Data);
		}
		else {
			outStream.Write ("I");
			outStream.WriteLine (tree.Data);
			RecWriteTree (tree.YesPtr, outStream);
			RecWriteTree (tree.NoPtr, outStream);
		}
	}
	
	// Try to open the data file and set up for the recursive method
	public void ReadTree ()
	{
		string fName = treePath;
		StreamReader inStream;
		try {
			inStream = new StreamReader (fName);
			root = RecReadTree (inStream);
			inStream.Close ();
		}
		catch (FileNotFoundException ex) {
			Debug.Log("There's no file, so I don't know anything.");
			return;
		}
	}
	
	// Recursive pre-order "traversal" that builds the tree as it traverses it, kind of...
	Node RecReadTree (StreamReader inStream)
	{
		try {
			string inBuffer = inStream.ReadLine ();                      // Next line from file
			string nodeType = inBuffer.Substring (0, 1);                 // First letter of line
			string phrase = inBuffer.Substring (1, inBuffer.Length - 1); // Rest of the line
			Node tree = new Node (phrase);                               // Make a node for phrase
			
			if (nodeType.Equals ("I")) {                   // Interior node => recurse
				tree.YesPtr = RecReadTree (inStream);
				tree.NoPtr = RecReadTree (inStream);
			}
			return tree;                                // Return the node we just built
		}
		catch (System.NullReferenceException ex) {
			Debug.Log("The file is empty, so I don't know anything.");
			return null;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

/* Node class - Nodes have a string and 2 child Nodes.
	 * The children are either both null (a leaf node => string is a name)
	 * or they're both non-null (an interior node => string is a phrase)
	 */
public class Node
{
	private string data = null;	// Used for demo to input a string to ask the user
	private Node noPtr = null;		// Followed if the Test() fails
	private Node yesPtr = null;		// Followed if the Test() succeeds
	
	// Getters and setters
	public string Data
	{
		get { return data; }
		set { data = value; }
	}
	
	public Node NoPtr
	{
		get { return noPtr; }
		set { noPtr = value; }
	}
	
	public Node YesPtr
	{
		get { return yesPtr; }
		set { yesPtr = value; }
	}
	
	public Node (string s)      // data string passed into constructor
	{
		data = s;
		noPtr = null;          // New nodes are leaf nodes
		yesPtr = null;
	}
	
	// Method that encapsulates the test for this node in the decision tree
	// For this demo, the "test" is to ask the user a question, treat a
	// yes answer as true and a no answer as false
	// In the general case this would be an arbitrary function call or logic
	// the implemented the test.

	/*
	public bool Test ()
	{
		Debug.Log(data + " ");
		bool answer = Console.ReadLine ();
		return answer.Equals ("y");
	}
	*/
}
