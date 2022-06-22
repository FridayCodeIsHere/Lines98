using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class Game : MonoBehaviour
{
    private int _score;
    private Button[,] _buttons;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Sprite[] _images;
    [SerializeField] private AudioSource _audio;
    private Lines _lines;
    private void Start()
    {
        _lines = new Lines(ShowBox, PlayCut);
        InitButtons();
        _lines.Start();
    }

    public void ShowBox(int x, int y, int value)
    {
        _buttons[x, y].GetComponent<Image>().sprite = _images[value];
    }

    public void PlayCut()
    {
        _audio.Play();
    }

    public void OnClick()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        int index = GetNumber(name);
        int x = index % Lines.SIZE;
        int y = index / Lines.SIZE;
        _lines.Click(x, y);
    }

    private void InitButtons()
    {
        _buttons = new Button[Lines.SIZE, Lines.SIZE];

        for (int i = 0; i < Lines.SIZE * Lines.SIZE; i++)
        {
            _buttons[i % Lines.SIZE, i / Lines.SIZE] = GameObject.Find($"Button ({i})").GetComponent<Button>();
        }
    }

    private int GetNumber(string name)
    {
        Regex regex = new Regex("\\((\\d+)\\)");
        Match match = regex.Match(name);
        if (!match.Success)
        {
            throw new Exception("Unrecognizxed object name");
        }
        Group group = match.Groups[1];
        string number = group.Value;
        return Convert.ToInt32(number);
    }
}
