using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person {

    string name;
    string sex;
    string number;
    string code;
    string description;
    
    public Person(string name, string sex, string number, string code, string description = "")
    {
        this.name = name;
        this.sex = sex;
        this.number = number;
        this.code = code;
        this.description = description;
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

    public string getDescription()
    {
        return description;
    }

    public string getCode()
    {
        return code;
    }
}
