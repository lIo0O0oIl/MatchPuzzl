using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour    //모노비헤이비어를 자진 것 만 상속받으려는거.
{
    static T m_instance;      //t의 인스턴스가 만들어짐

    public static T instance
    {
        get
        { 
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<T>();
                if (m_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);   //이름을 T로 만들어줌
                    m_instance = singleton.AddComponent<T>();    //T컴포넌트를 추가해줌.
                }
            }
            return m_instance;
        }
    }

    public virtual void Awake()     //가상 메서드
    {
        if (m_instance == null)
        {
            m_instance = this as T;     //T 타입으로 캐스팅 하는거
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 제네릭으로 싱글턴을 만들어 주는 이유는 각 메니저에다
    // 싱글턴을 만들었을 때 계속 싱글턴을 만드는 것을 방지하기 위해서 사용함.

}
