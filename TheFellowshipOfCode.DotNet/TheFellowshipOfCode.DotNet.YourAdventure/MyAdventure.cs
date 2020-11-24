using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTF2020.Contracts;
using HTF2020.Contracts.Enums;
using HTF2020.Contracts.Models;
using HTF2020.Contracts.Models.Adventurers;
using HTF2020.Contracts.Models.Enemies;
using HTF2020.Contracts.Models.Party;
using HTF2020.Contracts.Requests;
using HTF2020.GameController.State;

namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class MyAdventure : IAdventure
    {
        private readonly Random _random = new Random();
        private Party _party;
        public Task<Party> CreateParty(CreatePartyRequest request)
        {
            var party = new Party
            {
                Name = "Monke Hakkers",
                Members = new List<PartyMember>()
            };

            //charPoints dedicaten?


            /*
             *  Wizard = (this.Intelligence - 10) / 2 + new Random().Next(1, 6);
             * 
             *  Fighter = (this.Strength - 10) / 2 + new Random().Next(1, 6)
             *
             *
             */
            for (var i = 0; i < request.MembersCount; i++)
            {
                if (i % 2 == 0)
                {
                    party.Members.Add(new Wizard()
                    {
                        Id = i,
                        Name = $"Monke Wizard {i + 1}",
                        Constitution = 13,
                        Strength = 8,
                        Intelligence = 13
                    });
                }
                else
                {
                    party.Members.Add(new Fighter()
                    {
                        Id = i,
                        Name = $"Monke Fighter {i + 1}",
                        Constitution = 13,
                        Strength = 13,
                        Intelligence = 8
                    });
                }
               
            }

            _party = party;
            return Task.FromResult(party);
        }

        
        List<Turn> previousTurns = new List<Turn>();


        private bool calculated = false;


        public float EnemyGroupDifficulty(EnemyGroup enemies, Party party)
        {
            var totalCost = 0.00F;
            foreach (var enemy in enemies.Enemies)
            {

                if (enemy.GetType() == typeof(Goblin))
                {
                    totalCost += 2;
                }
                else if (enemy.GetType() == typeof(Sorcerer) || enemy.GetType() == typeof(Heavy))
                {
                    totalCost += 3;
                }
            }

            if (hasPotions) totalCost -= 2;

            return totalCost;


        }

        public bool hasPotions = false;

        public Stack<AstarNode> currentPath = null;

        List<List<AstarNode>> nodeList = new List<List<AstarNode>>();//map in astarnodes
        



        public Task<Turn> PlayTurn(PlayTurnRequest request)
        {
            //0,0 -> 0,9
            return riskBasedStrategic();

            Task<Turn> riskBasedStrategic()
            {
                var xLocation = request.PartyLocation.X; //X locatie partymember
                var yLocation = request.PartyLocation.Y; //Y locatie partymember
                var member = request.PartyMember;
                var map = request.Map; // De tilemap
                var possibleActions = request.PossibleActions; //Al de mogelijke acties dat het character kan doen
                var isInCombat = request.IsCombat;
                var possibleEnemies = request.PossibleTargets;
                if (possibleActions.Contains(TurnAction.DrinkPotion)) hasPotions = true;

                
               
                

                if (currentPath == null)
                {
                    for (int i = 0; i < map.Tiles.GetLength(1); i++)
                    {
                        nodeList.Add(new List<AstarNode>());
                        for (int j = 0; j < map.Tiles.GetLength(0); j++)
                        {
                            var tile = map.Tiles[j,i];

                            AstarNode.Walkable walkable;
                            switch (tile.TileType)
                            {
                                case TileType.Wall:
                                    walkable = AstarAlgorithm.Walkable.Not_Able;
                                    break;
                                default:
                                    walkable = AstarAlgorithm.Walkable.Able;
                                    break;
                                    
                            }

                            var weight = 0.00F;
                            if (tile.TileType == TileType.Enemy)
                            {
                                weight = EnemyGroupDifficulty(tile.EnemyGroup, _party);
                            }


                            nodeList[i].Add(new AstarNode(new Vector2(j, i), walkable, weight));
                           
                        }

                    } //Calculate MAP

                    //What are we going to do :thonk:
                    var destination = new Vector2(0, 9);
                    var astar = new AstarAlgorithm(nodeList);
                    currentPath = astar.FindPath(new Vector2(xLocation, yLocation), destination);

                    return Task.FromResult(new Turn(TurnAction.Pass));
                }

                var currentStep = currentPath.Pop();
                Console.WriteLine(currentStep.Position.x + " : " + currentStep.Position.y);


                return Task.FromResult(new Turn(TurnAction.Pass));

            }
        }
    }
} 