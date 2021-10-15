using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using System.Linq;


namespace MySboxGamemode
{
	public static class breadth-first search
	{

		/// <summary>
		/// </summary>
		/// <param name="Start">Начальная позиция в индексах комнаты которые пишутся в MapArrays[i]</param>
		/// <param name="Goal">Искомая позиция в индексах комнаты которые пишутся в MapArrays[i]</param>
		/// <param name="MapArrays">Берет конкретный массив и ищет по нему</param>

		public static List<Vector2> pathfind( Vector2 Start, Vector2 Goal, Dictionary<string, StateVector> MapArrays )
		{


			string ConvertedPosition;
			int UselessPoints = 0;

			StateVector DoorWays;
			Vector2 CurrentPosition = Start;
			List<Vector2> Points = new List<Vector2>( 10 );	
			List<Vector2> Branches = new List<Vector2>( 10 ); // список комнат развилок, чтобы можно было к ним вернуться.
			List<Vector2> BranchVis = new List<Vector2>( 10 ); // список комнат развилок, чтобы можно было к ним вернуться.
			List<Vector2> Visited = new List<Vector2>( 100 );

			Branches.Add( Start );
			Points.Add( CurrentPosition );

			// цикл, пока комната не имеет только 1 вход или это не goal - двигаемся по графу
			while ( Branches.Count > 0 && CurrentPosition != Goal ) // // 0 право 1 верх 2 лево 3 низ
			{
				uselessPoints++;


				if ( Visited.Contains( CurrentPosition ) && !Branches.Contains( CurrentPosition ) )
				{
					Points.Clear();
					Log.Info( "Обнулено" );
					uselessPoints = 0;

				}
				else
				{
					ConvertedPosition = Convert.ToString( CurrentPosition.x + ":" + CurrentPosition.y );
					if ( MapArrays.TryGetValue( ConvertedPosition, out DoorWays ) )
					{
						Log.Info( CurrentPosition + " | " + amountofsides( DoorWays ) + " | Branches Amount = " + Branches.Count() + " | Visited/Overall = " + Visited.Count + "/" + MapArrays.Count );

						if ( amountofsides( DoorWays ) < 2 )
						{

							CurrentPosition = Branches.Last();

							Log.Info( "Вернулись в ветку " + CurrentPosition );
							ConvertedPosition = Convert.ToString( CurrentPosition.x + ":" + CurrentPosition.y );
							if ( MapArrays.TryGetValue( ConvertedPosition, out DoorWays ) ) { } else { Log.Error( "Не найдена комната с индексом" + ConvertedPosition ); }
							Branches.Remove( Branches.Last() );

						}

						int sidesiterator = 0;
						if ( !BranchVis.Contains( CurrentPosition ) )
						{
							if ( Visited.Contains( moveright( CurrentPosition ) ) )
							{
								sidesiterator++;
							}
							if ( Visited.Contains( moveleft( CurrentPosition ) ) )
							{
								sidesiterator++;
							}
							if ( Visited.Contains( moveup( CurrentPosition ) ) )
							{
								sidesiterator++;
							}
							if ( Visited.Contains( movedown( CurrentPosition ) ) )
							{
								sidesiterator++;
							}

							while ( sidesiterator < (amountofsides( DoorWays ) - 1) )
							{
								sidesiterator++;
								Branches.Add( CurrentPosition );
								Log.Info( "Добавлена точка разветвления | " + CurrentPosition + " sidesiterator = " + sidesiterator + " | " + uselessPoints );

							}
							BranchVis.Add( CurrentPosition );
						}

						if ( (DoorWays.sides[0] == EState.Free || DoorWays.sides[0] == EState.Musthave) && !Visited.Contains( moveright( CurrentPosition ) ) )
						{
							Log.Info( "MoveRight" );
							Visited.Add( CurrentPosition );
							CurrentPosition = moveright( CurrentPosition );
						}
						else if ( (DoorWays.sides[1] == EState.Free || DoorWays.sides[1] == EState.Musthave) && !Visited.Contains( moveup( CurrentPosition ) ) )
						{
							Log.Info( "MoveUP" );
							Visited.Add( CurrentPosition );
							CurrentPosition = moveup( CurrentPosition );
						}
						else if ( (DoorWays.sides[2] == EState.Free || DoorWays.sides[2] == EState.Musthave) && !Visited.Contains( moveleft( CurrentPosition ) ) )
						{
							Log.Info( "MoveLeft" );
							Visited.Add( CurrentPosition );
							CurrentPosition = moveleft( CurrentPosition );
						}
						else if ( (DoorWays.sides[3] == EState.Free || DoorWays.sides[3] == EState.Musthave) && !Visited.Contains( movedown( CurrentPosition ) ) )
						{
							Log.Info( "MoveDown" );
							Visited.Add( CurrentPosition );
							CurrentPosition = movedown( CurrentPosition );
						}
						else
						{
							Visited.Add( CurrentPosition );
							Branches.Remove( CurrentPosition );
							CurrentPosition = Branches.Last();
							Log.Info( "Вернулись в ветку " + CurrentPosition );
						}
						if ( !Points.Contains( CurrentPosition ) )
						{
							Points.Add( CurrentPosition );
						}
					}

					else
					{
						Log.Error( "Не найдена комната с индексом" + ConvertedPosition );
						break;
					}
				}
			}

			int count = Points.Count - 2;
			for ( int i = 0; i < count; i++ )
			{
				Log.Info( "Checking Points" );
				if ( Math.Abs( Points[i].x - Points[i + 1].x ) >= 2 || Math.Abs( Points[i].y - Points[i + 1].y ) >= 2 )
				{

					Log.Info( "Removed: " + Points[i] );
					Points.Remove( Points[i] );
				}
			}
			return Points;
		}
	

		// Для читабельности
		static Vector2 moveright(Vector2 CurrentPosition)
		{
			CurrentPosition.x++;
			return CurrentPosition;
		}

		static Vector2 moveleft( Vector2 CurrentPosition )
		{
			CurrentPosition.x--;
			return CurrentPosition;
		}

		static Vector2 moveup( Vector2 CurrentPosition )
		{
			CurrentPosition.y++;
			return CurrentPosition;
		}

		static Vector2 movedown( Vector2 CurrentPosition )
		{
			CurrentPosition.y--;
			return CurrentPosition;
		}



		// Вспомогательные ф-ии:
		static int amountofsides( StateVector sv )
		{
			int count = 0;
			int i = 0;
			while ( i < 4 )
			{
				if ( sv.sides[i] == EState.Free || sv.sides[i] == EState.Musthave )
				{
					count++;
				}
				i++;
			}
			return count;
		}

			// Задел для A*
		private static float GetHeuristicPathLength( Vector2 from, Vector2 to )
		{
			return Math.Abs( from.x - to.x ) + Math.Abs( from.y - to.y );
		}

	}
}