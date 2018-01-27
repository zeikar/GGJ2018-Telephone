using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person {

    string name;
    string sex;
    string number;
    
    public Person(string name, string sex, string number)
    {
        this.name = name;
        this.sex = sex;
        this.number = number;
    }

    public string getName()
    {
        return name;
    }
    
    public string getSex()
    {
        return sex;
    }

    public string getNumber()
    {
        return number;
    }
}
