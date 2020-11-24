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

namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class MyAdventure : IAdventure
    {
        private readonly Random _random = new Random();
        private Party _party;


        public List<Point> currentPath = null;
        public List<Point> TreasurePoints = new List<Point>();
        public List<Point>
        public List<Point> BlacklistPoints = new List<Point>();
        public float[,] tilesmap;
        public Point FinishPoint;


        public bool hasPotions;
        public bool calculated = false;

        public Task<Party> CreateParty(CreatePartyRequest request)
        {
            var party = new Party
            {
                Name = "Monke Hakkers",
                Members = new List<PartyMember>()
            };

            for (var i = 0; i < request.MembersCount; i++)
                if (i % 2 == 0)
                    party.Members.Add(new Wizard
                    {
                        Id = i,
                        Name = $"Monke Wizard {i + 1}",
                        Constitution = 13,
                        Strength = 8,
                        Intelligence = 13
                    });
                else
                    party.Members.Add(new Fighter
                    {
                        Id = i,
                        Name = $"Monke Fighter {i + 1}",
                        Constitution = 13,
                        Strength = 13,
                        Intelligence = 8
                    });

            _party = party;
            return Task.FromResult(party);
        }


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
                if (possibleActions.Contains(TurnAction.Loot))
                {
                    BlacklistPoints.Add(new Point(xLocation, yLocation));
                    return Task.FromResult(new Turn(TurnAction.Loot));
                }

                if (!calculated)
                {
                    var width = map.Tiles.GetLength(0);
                    var height = map.Tiles.GetLength(1);
                    tilesmap = new float[width, height];

                    for (var i = 0; i < width; ++i)
                    for (var j = 0; j < height; ++j)
                    {
                        var tile = map.Tiles[i, j];
                        #region WeightCalculation
                        var weight = 0.00F;
                        if (tile.TileType == TileType.Enemy)
                            weight = EnemyGroupDifficulty(tile.EnemyGroup, _party);
                        else if (tile.TileType == TileType.Wall || tile.TerrainType == TerrainType.Water)
                            weight = 0.0f;
                        else
                            weight = 0.1f;
                        tilesmap[i, j] = weight;
                        #endregion

                        if (tile.TileType == TileType.TreasureChest)
                        {
                            TreasurePoints.Add(new Point(i, j));
                        }

                    }
                }

                if (currentPath == null)
                {
                    var grid = new Grid(tilesmap);
                    if (TreasurePoints.Count > 0)
                    {
                        var pointToGo = TreasurePoints[0];
                        TreasurePoints.RemoveAt(0);
                        if (!BlacklistPoints.Contains(pointToGo))
                        {
                            currentPath = Pathfinding.FindPath(grid, new Point(xLocation, yLocation), pointToGo, Pathfinding.DistanceType.Manhattan);
                        }
                    }
                    else
                    {

                    }
                }

                if (currentPath.Count > 0)
                {
                    var currentStep = currentPath[0];
                    currentPath.RemoveAt(0);
                    return Task.FromResult(new Turn(GetTurnAction(xLocation, yLocation, currentStep)));
                }
                else currentPath = null;


                return Task.FromResult(new Turn(TurnAction.Pass));
            }
        }

        public TurnAction GetTurnAction(int oX, int oY, Point comparisonPoint)
        {
            var cX = comparisonPoint.x;
            var cY = comparisonPoint.y;
            if (oX - cX > 0) return TurnAction.WalkWest;
            else if (oX - cX < 0) return TurnAction.WalkEast;
            else if (oY - cY > 0) return TurnAction.WalkNorth;
            else if (oY - cY < 0) return TurnAction.WalkSouth;
            else return TurnAction.Pass;
        }

        public float EnemyGroupDifficulty(EnemyGroup enemies, Party party)
        {
            var totalCost = 0.00F;
            foreach (var enemy in enemies.Enemies)
                if (enemy.GetType() == typeof(Goblin))
                    totalCost += 2;
                else if (enemy.GetType() == typeof(Sorcerer) || enemy.GetType() == typeof(Heavy)) totalCost += 3;

            if (hasPotions) totalCost -= 2;

            return totalCost;
        }
    }
}