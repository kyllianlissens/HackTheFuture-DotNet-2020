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

    public class TileEnemy
    {
        public Tile tile;
        public Enemy enemy;

        public TileEnemy(Tile Tile, Enemy Enemy)
        {
            tile = Tile;
            enemy = Enemy;
        }
    }

    public class TileLocation
    {
        public int x;
        public int y;
        public Tile tile;

        public TileLocation(int x, int y, Tile tile)
        {
            this.x = x;
            this.y = y;
            this.tile = tile;
        }
    }
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

        List<TileEnemy> enemies = new List<TileEnemy>();
        List<TileLocation> tiles = new List<TileLocation>();

        private bool calculated = false;

        public double RangeDiffrence(TileLocation t1, TileLocation t2)
        {
            return Math.Sqrt(Math.Pow((t2.x - t1.x), 2) + Math.Pow((t2.y - t1.y), 2));

        }

        public double EnemyGroupDifficulty(EnemyGroup enemies, Party party)
        {
            var totalCost = 0.00;
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

            totalCost -= amountOfPotions;

            return totalCost;


        }

        public int amountOfPotions = 0;

        public Task<Turn> PlayTurn(PlayTurnRequest request)
        {
            
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

                if (possibleActions.Contains(TurnAction.DrinkPotion)) amountOfPotions++;

                

                

                if (!calculated)
                {
                    calculated = true;
                    for (int i = 0; i < map.Tiles.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.Tiles.GetLength(1); j++)
                        {
                            var tile = map.Tiles[i, j];
                            tiles.Add(new TileLocation(i, j, tile));
                            if (tile.TileType == TileType.Enemy)
                            {
                                EnemyGroupDifficulty(tile.EnemyGroup, _party);
                                foreach (var tileEnemy in tile.EnemyGroup.Enemies.Select(enemy => new TileEnemy(tile, enemy)))
                                {
                                    enemies.Add(tileEnemy);
                                }
                            }
                           
                        }
                    }

                    return Task.FromResult(new Turn(TurnAction.Pass));
                }

                

                if (request.PossibleActions.Contains(TurnAction.Loot)) return Task.FromResult(new Turn(TurnAction.Loot));

                return Task.FromResult(new Turn(request.PossibleActions[_random.Next(request.PossibleActions.Length)]));

            }
        }
    }
} 