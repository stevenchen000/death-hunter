﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;


namespace BattleSystem
{
    public class BattleManager : MonoBehaviour
    {
        public List<CharacterManager> allCharacters;
        public List<CharacterManager> allEnemies;
        public List<CharacterManager> allAllies;

        public List<CharacterTurn> turnOrder;
        private List<CharacterTurn> allCharactersNextTurns;

        public CharacterTurn currentTurn;
        public int turnIndex = -1;
        public float turnTime = 0;
        public float turnTimer = 0;

        public AbilityElement fieldElement;
        public AbilityElement nullElement;

        [SerializeField]
        [Tooltip("Number of turns to calculate in turnOrder")]
        private int numberOfTurns = 10;



        private void Awake()
        {
            InitCharacter();
            InitFirstTurns();
            CalculateTurnOrder();
            InitTurn(turnOrder[0]);
        }

        private void Start()
        {
            for (int i = 0; i < allCharacters.Count; i++)
            {
                allCharacters[i].gameObject.SetActive(true);
            }
        }


        private void Update()
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                if (currentTurn.character.NotTakingAction())
                {
                    ChangeTurn();
                }
            }
        }


        private void InitTurn(CharacterTurn nextTurn) {
            currentTurn = nextTurn;
            turnTime = currentTurn.character.stats.attackTime;
            turnTimer = turnTime;

            nextTurn.character.StartTurn();
        }


        public void ChangeTurn()
        {
            turnOrder[0].turnTime = IncrementTurn(turnOrder[0]).turnTime;
            CalculateTurnOrder();

            InitTurn(turnOrder[0]);
        }


        private void InitCharacter()
        {
            CharacterManager[] characters = FindObjectsOfType<CharacterManager>();
            allCharacters = new List<CharacterManager>();
            allEnemies = new List<CharacterManager>();
            allAllies = new List<CharacterManager>();

            for (int i = 0; i < characters.Length; i++)
            {
                allCharacters.Add(characters[i]);

                if (characters[i].tag == "enemy")
                {
                    allEnemies.Add(characters[i]);
                }
                else
                {
                    allAllies.Add(characters[i]);
                }
            }
        }


        public void ChangeFieldElement(AbilityElement newElement)
        {
            if (newElement != nullElement)
            {
                fieldElement = newElement;
            }
        }

        public void ResetElement()
        {
            fieldElement = nullElement;
        }




        /// <summary>
        /// Initializes the first turns for all characters
        /// </summary>
        private void InitFirstTurns() {
            allCharactersNextTurns = new List<CharacterTurn>();

            for (int i = 0; i < allCharacters.Count; i++) {
                CharacterTurn nextTurn = new CharacterTurn(allCharacters[i], allCharacters[i].stats.CalculateFirstTurn());
                allCharactersNextTurns.Add(nextTurn);
            }
        }



        /// <summary>
        /// Calculates the turn order
        /// </summary>
        public void CalculateTurnOrder()
        {
            turnOrder = new List<CharacterTurn>();

            for (int i = 0; i < allCharactersNextTurns.Count; i++) {
                CharacterTurn nextTurn = allCharactersNextTurns[i];

                //On the first iteration, just populate the list as if the first character is the only one taking turns
                if (i == 0)
                {
                    turnOrder.Add(nextTurn);

                    while (turnOrder.Count < numberOfTurns)
                    {
                        nextTurn = IncrementTurn(nextTurn);
                        turnOrder.Add(nextTurn);
                    }
                }
                else { //On other iterations, loop through and insert turn if faster
                    for (int j = 0; j < numberOfTurns; j++) {
                        if (nextTurn.turnTime < turnOrder[j].turnTime)
                        {
                            turnOrder.Insert(j, nextTurn);
                            nextTurn = IncrementTurn(nextTurn);
                            turnOrder.RemoveAt(numberOfTurns);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the following turn for the character
        /// </summary>
        /// <param name="turn"></param>
        /// <returns></returns>
        private CharacterTurn IncrementTurn(CharacterTurn turn) {
            CharacterManager character = turn.character;
            float nextTurnTime = character.stats.CalculateNextTurn(turn.turnTime);

            return new CharacterTurn(character, nextTurnTime);
        }



        /// <summary>
        /// Adds delay to the character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="delay"></param>
        public void AddDelay(CharacterManager character, float delay) {
            for (int i = 0; i < allCharactersNextTurns.Count; i++){
                CharacterTurn nextTurn = allCharactersNextTurns[i];

                if (nextTurn.character == character) {
                    nextTurn.turnTime += delay;
                    CalculateTurnOrder();
                    break;
                }
            }
        }
    }
}