using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour    //�������̺� ���� �� �� ��ӹ������°�.
{
    static T m_instance;      //t�� �ν��Ͻ��� �������

    public static T instance
    {
        get
        { 
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<T>();
                if (m_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);   //�̸��� T�� �������
                    m_instance = singleton.AddComponent<T>();    //T������Ʈ�� �߰�����.
                }
            }
            return m_instance;
        }
    }

    public virtual void Awake()     //���� �޼���
    {
        if (m_instance == null)
        {
            m_instance = this as T;     //T Ÿ������ ĳ���� �ϴ°�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���׸����� �̱����� ����� �ִ� ������ �� �޴�������
    // �̱����� ������� �� ��� �̱����� ����� ���� �����ϱ� ���ؼ� �����.

}
