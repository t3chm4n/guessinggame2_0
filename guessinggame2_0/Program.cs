using System;
using System.Collections.Generic;
using System.Linq;                 
using System.Windows.Forms;

//ToDo List:
/*
  - some minor data duplication (question in Main(), linq lambdas...
  - try to catch some expections, especially if we plan to use this classes in future (other games maybe...)
  - include unit tests (NUnit + OpenCover)
  - create some interfaces and improve classes and methods, thinking in future and generic uses
  - keep data in a database (animalkinator?) :)
*/



namespace guessinggame2_0
{

    class Animal
    {
        public string animal { get; }
        public List<string> attributes { get; } = new List<string>();

        public Animal(string animal, string attribute)
        {
            this.animal = animal;
            this.attributes.Add(attribute);
        }

        public void addAttribute(string attribute)
        {
            this.attributes.Add(attribute);
        }

    }

    class Program
    {
        //choose question based on binary search (try to find the attribute closer to 50% of the distribution
        static Dictionary<string, string> ChooseQuestion(List<Animal> animals)
        {
            //mount the list of attributes
            Dictionary<string, int> countAttributes = new Dictionary<string, int>();
            foreach (Animal animal in animals)
            {
                foreach (string attrib in animal.attributes)
                {
                    if (attrib != "")
                    {
                        if (countAttributes.ContainsKey(attrib))
                        {
                            countAttributes[attrib]++;
                        }
                        else
                        {
                            countAttributes.Add(attrib, 1);
                        }
                    }
                }
            }

            //try to find the closest to the middle in distribution (aka binary search)
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (countAttributes.Count > 1)
            {
                int middle = animals.Count / 2;
                int closest = countAttributes.Values.Aggregate((x, y) => Math.Abs(x - middle) < Math.Abs(y - middle) ? x : y);
                result.Add("attribute", countAttributes.FirstOrDefault(x => x.Value == closest).Key);
                return result;
            }
            else //or return animal, if has only one attribute left
            {
                result.Add("animal", animals.First().animal);
                return result;
            }

        }

        //RefineList removes the animals accordind to user answers
        static List<Animal> RefineList(List<Animal> animals, Dictionary<string, string> question, int response) //0=delete; 1=mantain
        {   
            List<Animal> subListOfAnimals = new List<Animal>();
            foreach (Animal animal in animals)
            {
                if ((animal.attributes.Contains(question.First().Value) && response == 1) ||
                    (!(animal.attributes.Contains(question.First().Value)) && response == 0))
                {
                    if (animal.attributes.Count > 1)
                    {
                        //removes attributes already questioned to user
                        animal.attributes.Remove(question.First().Value);
                    }
                    if (animal.attributes.Count > 0) {
                        subListOfAnimals.Add(animal);
                    }
                    
                }
            }
            return subListOfAnimals;
        }

        //Include attributes or animal+attributes according to user answers
        static void SetAnimal(string key, List<Animal> animals, List<string> attributes)
        {
            string animal = Microsoft.VisualBasic.Interaction.InputBox("What was the animal that you thought about?",
                "Guessing Game");
            string attribute = Microsoft.VisualBasic.Interaction.InputBox("A " + animal + " __________ but a " +
                key + " does not (Fills it with an animal trait, like 'lives in water')", "Guessing Game");
            if (animals.FirstOrDefault(x => x.animal.Equals(animal)) != null)
            {       
                animals.FirstOrDefault(x => x.animal.Equals(animal)).addAttribute(attribute);
            }
            else
            {
                animals.Add(new Animal(animal, attribute));
            }

            //includes attributes answered "Yes" by user
            if (attributes.Count > 0)
            {
                foreach (string attrib in attributes)
                {
                    if (animals.FirstOrDefault(x => x.animal.Equals(animal)).attributes.FirstOrDefault(y => y.Equals(attrib)) == null)
                    {
                        animals.FirstOrDefault(x => x.animal.Equals(animal)).addAttribute(attrib);
                    }
                }
            }
        }

        static void Main()
        {
            //default set
            List<Animal> animals = new List<Animal>();
            animals.Add(new Animal("shark", "lives in water"));
            animals.Add(new Animal("monkey", "lives in forest"));

            while (MessageBox.Show("Think about an animal...", "Guessing Game", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {                                              
                List<string> attributes = new List<string>();
                List<Animal> subListOfAnimals = animals.ToList();

                bool success = false;  
                              
                while (!success)
                {         
                    Dictionary<string, string> question = ChooseQuestion(subListOfAnimals);

                    //if only one attribute/animal is left...
                    if (question.First().Key.Equals("animal"))
                    {
                        if (MessageBox.Show("Does the animal that you thought about a " + question.First().Value + "?",
                        "Guessing Game", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            MessageBox.Show("I win again!");
                            success = true;
                            break;
                        }
                        else
                        {
                            SetAnimal(question.First().Value, animals, attributes);
                            success = true;
                        }
                    }
                    else //or ask an attribute question
                    {
                        if (MessageBox.Show("Does the animal that you thought about " + question.First().Value + "?",
                        "Guessing Game", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            attributes.Add(question.First().Value); //keep attribute for SetAnimal later
                            subListOfAnimals = RefineList(animals, question, 1); //keeps animals with that attribute
                        }
                        else
                        {    
                            subListOfAnimals = RefineList(animals, question, 0); //remove animals with that attribute
                        }            
                    }                                                   
                }
            }                                        
        }
    }
}
