using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Класс, отвечающий за отображение меню паузы и взаимодействия с этим меню. </summary>
public class TabMenu : MonoBehaviour
{
    [Header("UI Objects")]
    /// <summary> Список кнопок внутри меню</summary>
    [SerializeField] private List<Button> _tabButtons;

    /// <summary>Список информационных блоков для соответствующих кнопок меню. </summary>
    [SerializeField] private List<GameObject> _tabInfo;

    [Header("Sounds")]
    /// <summary> Аудиоисточник. </summary>
    [SerializeField] private AudioSource _audioSource;

    /// <summary> Звук нажатия на кнопку меню. </summary>
    [SerializeField] private AudioClip _pressedTabSound;

    /// <summary> Переменная запоминающая, какая кнопка была нажата. </summary>
    private int _previousIndex;

    private void Awake()
    {
        _audioSource.pitch = 1;
    }

    /// <summary>Привязка открытия информационных блоков к соответствующим кнопкам.. </summary>
    private void Start()
    {
        for (int i = 0; i < _tabButtons.Count; i++)
        {
            int _index = i;
            _tabButtons[i].onClick.AddListener(() => OnTabSelected(_index));
        }

        _previousIndex = (int)(_tabButtons.Count / 2);
        OnTabSelected(_previousIndex);
    }

    /// <summary> Обработка перемещения по кнопкам меню с помощью клавиатуры. </summary>
    private void Update()
    {
        bool _left = Input.GetKeyDown(KeyCode.Q);
        bool _right = Input.GetKeyDown(KeyCode.E);

        if (_left || _right)
        {
            int _direction = _left ? -1 : 1;
            OnTabSelected((_previousIndex + _tabButtons.Count + _direction) % _tabButtons.Count);
        }
    }

    /// <summary> Метод, вызываемый при нажатии на кнопку, открывает соответствующий блок и скрывает предыдущий. </summary>
    /// <param name="index">Индекс открываемого блока. </param>
    private void OnTabSelected(int index)
    {
        _tabInfo[_previousIndex].SetActive(false);
        _tabInfo[index].SetActive(true);
        _previousIndex = index;

        _audioSource.PlayOneShot(_pressedTabSound);
        _tabButtons[index].Select();
    }
}