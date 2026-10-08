using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void BaseTestMember()
    {
        // Use the Assert class to test conditions
        Enemy_State baseline=new Enemy_State();
        Enemy_State attack_test=new Attack_State();
        Enemy_State chase_test=new Chase_State();

        string a=baseline.Tick();

        string b=attack_test.Tick();
        string c=chase_test.Tick();
        Assert.AreNotEqual(a,b);
       
        Assert.AreNotEqual(a,c);


    }
    [Test]
        public void Fail_Test()
    {
        // Use the Assert class to test conditions
        Enemy_State baseline=new Enemy_State();
        Enemy_State attack_test=new Enemy_State(); 
    

        string a=baseline.Tick();

        string b=attack_test.Tick();//a==b
        
       
       //Assert.AreNotEqual(a,b);// test with fail
       Assert.AreEqual(a,b); // test with success
    }



    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.

}
