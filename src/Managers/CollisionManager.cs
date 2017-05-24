﻿using System;
using System.Diagnostics;
using SwinGameSDK;

namespace MyGame
{
	public class CollisionManager
	{
		TeamManager manager;
		GameManager gameManager;
		HealthManager healthManager;
		Random random = new Random ();
		HomeTower home;
		EnemyTower enemy;
		private int delay = 30;
		public CollisionManager (GameManager gameManager,TeamManager manager, HealthManager healthManager, HomeTower home, EnemyTower enemy)
		{
			this.gameManager = gameManager;
			this.home = home;
			this.enemy = enemy;
			this.manager = manager;
			this.healthManager = healthManager;
		}
		public void handleCollisions () {
			handleCollisionBetweenEntities ();
			handleCollisionsBetweenTowerAndEntity ();
		}
		private void handleCollisionBetweenEntities ()
		{
			if (manager.heros.Count > 0 && manager.enemies.Count > 0) {
				foreach (Unit heros in manager.heros) {
					foreach (Unit enemies in manager.enemies) {
						if (detectCollision (heros, enemies)) {//Spoof collision detection
							int hit = random.Next (2);//Battle interaction - who hit who
							if (hit == 0) {
								float dmg = heros.Dmg;
								healthManager.handleUnitDamage (enemies, dmg);
								enemies.SetLocation (SwinGame.SpriteX (enemies.Spirte) + 30.0f, Position.ENEMY_SPAWN_Y);//Knockback
							} else {
								float dmg = enemies.Dmg;
								healthManager.handleUnitDamage (heros, dmg);
								heros.SetLocation (SwinGame.SpriteX (heros.Spirte) - 30.0f, Position.HERO_SPAWN_Y);
							}
						}
						if (heros.getName () == "mage") {
							Mage mage = heros as Mage;
							if (mage.inRange (enemies)) {
								mage.Cast ();
								if (SwinGame.SpriteDX (mage.Spirte) != 0) {
									SwinGame.SpriteSetDX (mage.Spirte, 0);
								}
								if (detectCollision (mage.fireball, enemies)) {
									enemies.SetLocation (SwinGame.SpriteX (enemies.Spirte) + 15.0f, Position.ENEMY_SPAWN_Y);
									mage.fireball = null;
									healthManager.handleUnitDamage (enemies, mage.SpellDmg);

								}
							}else if (!mage.inRange (enemies)) {
								if (SwinGame.SpriteDX (mage.Spirte) == 0) {
									Console.WriteLine ("set to 0.4");
									SwinGame.SpriteSetDX (mage.Spirte, 0.4f);
								}
							}

						}
					}
				}
			}
		}
		private void handleCollisionsBetweenTowerAndEntity () {
				if (manager.enemies.Count > 0 && manager.heros.Count == 0) {
					Console.WriteLine ("Called 1");
					foreach (Unit enemies in manager.enemies) {
						if (detectCollision (enemies, home, MovementDirection.Left)) {
							SwinGame.SpriteSetDX (enemies.Spirte, 0);
							healthManager.handleTowerDamage (home, enemies.Dmg * 0.25f);
							if (home.Health <= 0) {
								gameManager.State = GameState.Ended;
							}
						}
					}
				} 

				if(manager.enemies.Count == 0 && manager.heros.Count > 0) {
					foreach (Unit heros in manager.heros) {
						if (heros.getName () == "mage") {
							Mage mage = heros as Mage;
							if (mage.inRange (enemy)) {
								mage.Cast ();
								Console.WriteLine (mage.fireball.Sprite.X);
								if (SwinGame.SpriteDX (mage.Spirte) != 0) {
									SwinGame.SpriteSetDX (mage.Spirte, 0);
								}
								if (detectCollision (mage.fireball, enemy)) {
									mage.fireball = null;
									healthManager.handleTowerDamage (enemy, mage.SpellDmg);

								}
							} else if (!mage.inRange (enemy)) {
								if (SwinGame.SpriteDX (mage.Spirte) == 0) {
									SwinGame.SpriteSetDX (mage.Spirte, 0.4f);
								}
							}
						}
							if (detectCollision (heros, enemy, MovementDirection.Right)) {
								SwinGame.SpriteSetDX (heros.Spirte, 0);
								healthManager.handleTowerDamage (enemy, heros.Dmg * 0.10f);
								if (enemy.Health <= 0) {
									gameManager.State = GameState.Ended;
							}
						}
					}
				}
		}

		public bool detectCollision (Unit hero, Unit enemy) {
			return hero.getX () + 12 >= enemy.getX ();
		}
		public bool detectCollision (Unit unit, Tower tower, MovementDirection direction)
		{
			if (direction == MovementDirection.Left) {
				return unit.getX () <= tower.X + 50;
			} else {
				return unit.getX () + 45 >= tower.X;
			}
		}
		public bool detectCollision (Projectile projectile,  Unit enemy)
		{
			return projectile.X+20 >= enemy.getX ();
		}
		public bool detectCollision (Projectile friendly, Projectile hostile)
		{
			return friendly.X >= hostile.X;
		}
		public bool detectCollision (Projectile projectile, Tower tower)
		{
			return projectile.X+50 >= tower.X;
		}

		
		}
	}
