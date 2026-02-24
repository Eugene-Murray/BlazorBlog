/// <reference types="phaser" />
/// <reference path="SpaceShooterGame.d.ts" />

interface GameOverData {
    score: number;
    level: number;
}

interface StarLayer extends Phaser.GameObjects.Image {
    speed: number;
}

interface EnemySprite extends Phaser.Physics.Arcade.Image {
    enemyType: string;
}

if (!window.SpaceShooter) {
    class SpaceShooterGame {
        private containerId: string;
        private game: Phaser.Game | null;

        constructor(containerId: string) {
            this.containerId = containerId;
            this.game = null;
        }

        init(): Phaser.Game {
            const config: Phaser.Types.Core.GameConfig = {
                type: Phaser.AUTO,
                width: 800,
                height: 600,
                parent: this.containerId,
                backgroundColor: '#0a0a1a',
                physics: {
                    default: 'arcade',
                    arcade: { gravity: { x: 0, y: 0 }, debug: false }
                },
                scene: [BootScene, MainMenuScene, GameScene, GameOverScene]
            };
            this.game = new Phaser.Game(config);
            return this.game;
        }

        destroy(): void {
            if (this.game) {
                this.game.destroy(true);
                this.game = null;
            }
        }
    }

    class BootScene extends Phaser.Scene {
        constructor() {
            super({ key: 'BootScene' });
        }

        preload(): void {
            const progressBar = this.add.graphics();
            const progressBox = this.add.graphics();
            progressBox.fillStyle(0x222222, 0.8);
            progressBox.fillRect(240, 270, 320, 50);
            this.load.on('progress', (value: number) => {
                progressBar.clear();
                progressBar.fillStyle(0x00ff00, 1);
                progressBar.fillRect(250, 280, 300 * value, 30);
            });
            this.load.on('complete', () => {
                progressBar.destroy();
                progressBox.destroy();
            });
        }

        create(): void {
            this.createPlayerShip();
            this.createEnemyShips();
            this.createBullets();
            this.createExplosionParticles();
            this.createPowerUps();
            this.createStars();
            this.createShield();
            this.scene.start('MainMenuScene');
        }

        private createPlayerShip(): void {
            const g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0x00aaff, 1);
            g.beginPath();
            g.moveTo(25, 0);
            g.lineTo(50, 45);
            g.lineTo(35, 35);
            g.lineTo(35, 50);
            g.lineTo(25, 45);
            g.lineTo(15, 50);
            g.lineTo(15, 35);
            g.lineTo(0, 45);
            g.closePath();
            g.fillPath();
            g.fillStyle(0x00ffff, 1);
            g.fillEllipse(25, 20, 8, 12);
            g.fillStyle(0xff6600, 1);
            g.fillRect(20, 45, 10, 5);
            g.lineStyle(2, 0x0088cc, 1);
            g.lineBetween(25, 5, 25, 40);
            g.lineBetween(10, 38, 25, 25);
            g.lineBetween(40, 38, 25, 25);
            g.generateTexture('player', 50, 55);
        }

        private createEnemyShips(): void {
            let g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xff3333, 1);
            g.beginPath();
            g.moveTo(20, 40);
            g.lineTo(0, 10);
            g.lineTo(20, 0);
            g.lineTo(40, 10);
            g.closePath();
            g.fillPath();
            g.fillStyle(0xff9999, 1);
            g.fillCircle(20, 18, 6);
            g.generateTexture('enemy1', 40, 45);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xff6600, 1);
            g.fillTriangle(15, 0, 0, 35, 30, 35);
            g.fillStyle(0xffcc00, 1);
            g.fillCircle(15, 15, 5);
            g.generateTexture('enemy2', 30, 40);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0x9933ff, 1);
            g.fillRect(10, 0, 40, 50);
            g.fillTriangle(30, 50, 10, 50, 20, 60);
            g.fillTriangle(30, 50, 50, 50, 40, 60);
            g.fillStyle(0x6600cc, 1);
            g.fillRect(0, 15, 60, 10);
            g.fillStyle(0xff00ff, 1);
            g.fillCircle(30, 25, 8);
            g.generateTexture('enemy3', 60, 65);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xff0000, 1);
            g.fillRect(20, 10, 60, 80);
            g.fillTriangle(50, 0, 20, 10, 80, 10);
            g.fillStyle(0xcc0000, 1);
            g.fillRect(0, 30, 100, 20);
            g.fillRect(0, 60, 100, 20);
            g.fillStyle(0xff6666, 1);
            g.fillCircle(50, 45, 15);
            g.fillStyle(0xffff00, 1);
            g.fillCircle(15, 40, 5);
            g.fillCircle(85, 40, 5);
            g.fillCircle(15, 70, 5);
            g.fillCircle(85, 70, 5);
            g.generateTexture('boss', 100, 100);
        }

        private createBullets(): void {
            let g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0x00ffff, 1);
            g.fillRect(2, 0, 4, 15);
            g.fillStyle(0xffffff, 1);
            g.fillRect(3, 0, 2, 15);
            g.generateTexture('playerBullet', 8, 15);

            g = this.make.graphics({ x: 0, y: 0 });
            g.fillStyle(0xff00ff, 1);
            g.fillRect(0, 0, 6, 20);
            g.fillStyle(0xffffff, 1);
            g.fillRect(2, 0, 2, 20);
            g.generateTexture('playerBulletPowered', 6, 20);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xff3333, 1);
            g.fillCircle(5, 5, 5);
            g.fillStyle(0xff9999, 1);
            g.fillCircle(5, 5, 2);
            g.generateTexture('enemyBullet', 10, 10);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xffff00, 1);
            g.fillCircle(8, 8, 8);
            g.fillStyle(0xffffff, 1);
            g.fillCircle(8, 8, 4);
            g.generateTexture('bossBullet', 16, 16);
        }

        private createExplosionParticles(): void {
            const g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xffffff, 1);
            g.fillCircle(4, 4, 4);
            g.generateTexture('particle', 8, 8);

            const e = this.make.graphics({ x: 0, y: 0,  });
            e.fillStyle(0xff6600, 1);
            e.fillCircle(20, 20, 20);
            e.fillStyle(0xffff00, 1);
            e.fillCircle(20, 20, 12);
            e.fillStyle(0xffffff, 1);
            e.fillCircle(20, 20, 5);
            e.generateTexture('explosion', 40, 40);
        }

        private createPowerUps(): void {
            let g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0x00ff00, 1);
            g.fillCircle(15, 15, 15);
            g.fillStyle(0xffffff, 1);
            g.fillRect(12, 5, 6, 20);
            g.fillRect(5, 12, 20, 6);
            g.generateTexture('powerupHealth', 30, 30);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xff00ff, 1);
            g.fillCircle(15, 15, 15);
            g.fillStyle(0xffffff, 1);
            g.fillTriangle(15, 5, 5, 25, 25, 25);
            g.generateTexture('powerupWeapon', 30, 30);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0x00ffff, 1);
            g.fillCircle(15, 15, 15);
            g.fillStyle(0xffffff, 1);
            g.lineStyle(3, 0xffffff, 1);
            g.strokeCircle(15, 15, 8);
            g.generateTexture('powerupShield', 30, 30);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xffff00, 1);
            g.fillCircle(15, 15, 15);
            g.fillStyle(0x000000, 1);
            g.fillRect(7, 12, 6, 6);
            g.fillRect(17, 12, 6, 6);
            g.generateTexture('powerupScore', 30, 30);
        }

        private createStars(): void {
            let g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xffffff, 1);
            g.fillCircle(1, 1, 1);
            g.generateTexture('starSmall', 2, 2);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xaaaaff, 1);
            g.fillCircle(2, 2, 2);
            g.generateTexture('starMedium', 4, 4);

            g = this.make.graphics({ x: 0, y: 0,  });
            g.fillStyle(0xffffaa, 1);
            g.fillCircle(3, 3, 3);
            g.generateTexture('starLarge', 6, 6);
        }

        private createShield(): void {
            const g = this.make.graphics({ x: 0, y: 0,  });
            g.lineStyle(3, 0x00ffff, 0.7);
            g.strokeCircle(35, 35, 32);
            g.lineStyle(2, 0x00ffff, 0.4);
            g.strokeCircle(35, 35, 28);
            g.generateTexture('shield', 70, 70);
        }
    }

    class MainMenuScene extends Phaser.Scene {
        constructor() {
            super({ key: 'MainMenuScene' });
        }

        create(): void {
            this.createStarfield();
            const title = this.add.text(400, 120, 'SPACE SHOOTER', {
                fontSize: '64px', fontFamily: 'Arial Black, sans-serif',
                color: '#00ffff', stroke: '#0088ff', strokeThickness: 6
            }).setOrigin(0.5);
            this.tweens.add({
                targets: title, scaleX: 1.05, scaleY: 1.05,
                duration: 1000, yoyo: true, repeat: -1, ease: 'Sine.easeInOut'
            });
            this.add.text(400, 190, 'DEFEND THE GALAXY', {
                fontSize: '24px', fontFamily: 'Arial, sans-serif', color: '#ff6600'
            }).setOrigin(0.5);
            this.createButton(400, 300, 'START GAME', () => this.scene.start('GameScene'));
            this.add.text(400, 420, 'CONTROLS', {
                fontSize: '28px', fontFamily: 'Arial, sans-serif', color: '#ffffff'
            }).setOrigin(0.5);
            this.add.text(400, 470, 'ARROW KEYS or WASD - Move', {
                fontSize: '18px', fontFamily: 'Arial, sans-serif', color: '#aaaaaa'
            }).setOrigin(0.5);
            this.add.text(400, 500, 'SPACE - Fire', {
                fontSize: '18px', fontFamily: 'Arial, sans-serif', color: '#aaaaaa'
            }).setOrigin(0.5);
            this.add.text(400, 530, 'P - Pause', {
                fontSize: '18px', fontFamily: 'Arial, sans-serif', color: '#aaaaaa'
            }).setOrigin(0.5);
            const highScore = localStorage.getItem('spaceShooterHighScore') || '0';
            this.add.text(400, 580, 'HIGH SCORE: ' + highScore, {
                fontSize: '20px', fontFamily: 'Arial, sans-serif', color: '#ffff00'
            }).setOrigin(0.5);
            const demoShip = this.add.image(650, 350, 'player').setScale(1.5);
            this.tweens.add({
                targets: demoShip, y: 380, duration: 2000,
                yoyo: true, repeat: -1, ease: 'Sine.easeInOut'
            });
        }

        private createStarfield(): void {
            for (let i = 0; i < 100; i++) {
                const x = Phaser.Math.Between(0, 800);
                const y = Phaser.Math.Between(0, 600);
                const size = Phaser.Math.Between(1, 3);
                const alpha = Phaser.Math.FloatBetween(0.3, 1);
                const star = this.add.circle(x, y, size, 0xffffff, alpha);
                this.tweens.add({
                    targets: star, alpha: alpha * 0.3,
                    duration: Phaser.Math.Between(1000, 3000), yoyo: true, repeat: -1
                });
            }
        }

        private createButton(x: number, y: number, text: string, callback: () => void): Phaser.GameObjects.Container {
            const button = this.add.container(x, y);
            const bg = this.add.rectangle(0, 0, 250, 60, 0x0044aa, 0.8).setStrokeStyle(3, 0x00ffff);
            const label = this.add.text(0, 0, text, {
                fontSize: '28px', fontFamily: 'Arial, sans-serif', color: '#ffffff'
            }).setOrigin(0.5);
            button.add([bg, label]);
            button.setSize(250, 60);
            button.setInteractive({ useHandCursor: true });
            button.on('pointerover', () => {
                bg.setFillStyle(0x0066cc, 1);
                this.tweens.add({ targets: button, scaleX: 1.1, scaleY: 1.1, duration: 100 });
            });
            button.on('pointerout', () => {
                bg.setFillStyle(0x0044aa, 0.8);
                this.tweens.add({ targets: button, scaleX: 1, scaleY: 1, duration: 100 });
            });
            button.on('pointerdown', callback);
            return button;
        }
    }

    class GameScene extends Phaser.Scene {
        private score!: number;
        private lives!: number;
        private level!: number;
        private weaponLevel!: number;
        private shieldActive!: boolean;
        private scoreMultiplier!: number;
        private isPaused!: boolean;
        private bossSpawned!: boolean;
        private enemiesKilled!: number;
        private enemiesForBoss!: number;
        private starLayers!: StarLayer[];
        private player!: Phaser.Physics.Arcade.Image;
        private playerShield!: Phaser.GameObjects.Image;
        private engineTrail!: Phaser.GameObjects.Particles.ParticleEmitter;
        private playerBullets!: Phaser.Physics.Arcade.Group;
        private enemyBullets!: Phaser.Physics.Arcade.Group;
        private enemies!: Phaser.Physics.Arcade.Group;
        private bosses!: Phaser.Physics.Arcade.Group;
        private powerups!: Phaser.Physics.Arcade.Group;
        private explosionEmitter!: Phaser.GameObjects.Particles.ParticleEmitter;
        private cursors!: Phaser.Types.Input.Keyboard.CursorKeys;
        private wasd!: { up: Phaser.Input.Keyboard.Key; down: Phaser.Input.Keyboard.Key; left: Phaser.Input.Keyboard.Key; right: Phaser.Input.Keyboard.Key };
        private spaceKey!: Phaser.Input.Keyboard.Key;
        private pauseKey!: Phaser.Input.Keyboard.Key;
        private lastFired!: number;
        private fireRate!: number;
        private spawnTimer!: Phaser.Time.TimerEvent;
        private scoreText!: Phaser.GameObjects.Text;
        private livesText!: Phaser.GameObjects.Text;
        private levelText!: Phaser.GameObjects.Text;
        private weaponText!: Phaser.GameObjects.Text;
        private multiplierText!: Phaser.GameObjects.Text;
        private bossHealthBar!: Phaser.GameObjects.Graphics;
        private pauseOverlay!: Phaser.GameObjects.Container;

        constructor() {
            super({ key: 'GameScene' });
        }

        init(): void {
            this.score = 0;
            this.lives = 3;
            this.level = 1;
            this.weaponLevel = 1;
            this.shieldActive = false;
            this.scoreMultiplier = 1;
            this.isPaused = false;
            this.bossSpawned = false;
            this.enemiesKilled = 0;
            this.enemiesForBoss = 20;
        }

        create(): void {
            this.createScrollingStarfield();
            this.createPlayer();
            this.playerBullets = this.physics.add.group({ classType: Phaser.Physics.Arcade.Image, maxSize: 50, runChildUpdate: true });
            this.enemyBullets = this.physics.add.group({ classType: Phaser.Physics.Arcade.Image, maxSize: 100, runChildUpdate: true });
            this.enemies = this.physics.add.group();
            this.bosses = this.physics.add.group();
            this.powerups = this.physics.add.group();
            this.explosionEmitter = this.add.particles(0, 0, 'particle', {
                speed: { min: 50, max: 200 }, scale: { start: 1, end: 0 },
                lifespan: 500, blendMode: 'ADD', emitting: false
            });
            this.setupCollisions();
            this.setupControls();
            this.createUI();
            this.spawnTimer = this.time.addEvent({
                delay: 2000, callback: this.spawnEnemy, callbackScope: this, loop: true
            });
            this.time.addEvent({
                delay: 30000, callback: this.increaseDifficulty, callbackScope: this, loop: true
            });
        }

        private createScrollingStarfield(): void {
            this.starLayers = [];
            const layers = [
                { count: 50, speed: 50, texture: 'starSmall' },
                { count: 30, speed: 100, texture: 'starMedium' },
                { count: 15, speed: 150, texture: 'starLarge' }
            ];
            layers.forEach(layer => {
                for (let i = 0; i < layer.count; i++) {
                    const star = this.add.image(
                        Phaser.Math.Between(0, 800),
                        Phaser.Math.Between(0, 600),
                        layer.texture
                    ) as StarLayer;
                    star.speed = layer.speed;
                    this.starLayers.push(star);
                }
            });
        }

        private createPlayer(): void {
            this.player = this.physics.add.image(400, 500, 'player');
            this.player.setCollideWorldBounds(true);
            this.player.setDepth(10);
            this.playerShield = this.add.image(400, 500, 'shield');
            this.playerShield.setVisible(false);
            this.playerShield.setDepth(11);
            this.engineTrail = this.add.particles(0, 0, 'particle', {
                speed: { min: 20, max: 50 }, scale: { start: 0.5, end: 0 },
                lifespan: 300, blendMode: 'ADD', tint: [0xff6600, 0xff9900, 0xffcc00],
                follow: this.player, followOffset: { x: 0, y: 25 }
            });
        }

        private setupControls(): void {
            this.cursors = this.input.keyboard!.createCursorKeys();
            this.wasd = this.input.keyboard!.addKeys({
                up: Phaser.Input.Keyboard.KeyCodes.W,
                down: Phaser.Input.Keyboard.KeyCodes.S,
                left: Phaser.Input.Keyboard.KeyCodes.A,
                right: Phaser.Input.Keyboard.KeyCodes.D
            }) as { up: Phaser.Input.Keyboard.Key; down: Phaser.Input.Keyboard.Key; left: Phaser.Input.Keyboard.Key; right: Phaser.Input.Keyboard.Key };
            this.spaceKey = this.input.keyboard!.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE);
            this.pauseKey = this.input.keyboard!.addKey(Phaser.Input.Keyboard.KeyCodes.P);
            this.lastFired = 0;
            this.fireRate = 150;
            this.pauseKey.on('down', () => this.togglePause());
        }

        private setupCollisions(): void {
            this.physics.add.overlap(this.playerBullets, this.enemies, this.bulletHitEnemy as Phaser.Types.Physics.Arcade.ArcadePhysicsCallback, undefined, this);
            this.physics.add.overlap(this.playerBullets, this.bosses, this.bulletHitBoss as Phaser.Types.Physics.Arcade.ArcadePhysicsCallback, undefined, this);
            this.physics.add.overlap(this.player, this.enemyBullets, this.playerHit as Phaser.Types.Physics.Arcade.ArcadePhysicsCallback, undefined, this);
            this.physics.add.overlap(this.player, this.enemies, this.enemyCollidePlayer as Phaser.Types.Physics.Arcade.ArcadePhysicsCallback, undefined, this);
            this.physics.add.overlap(this.player, this.powerups, this.collectPowerup as Phaser.Types.Physics.Arcade.ArcadePhysicsCallback, undefined, this);
        }

        private createUI(): void {
            this.scoreText = this.add.text(16, 16, 'SCORE: 0', {
                fontSize: '24px', fontFamily: 'Arial, sans-serif', color: '#ffffff',
                stroke: '#000000', strokeThickness: 2
            }).setDepth(100);
            this.livesText = this.add.text(16, 50, 'LIVES: 3', {
                fontSize: '20px', fontFamily: 'Arial, sans-serif', color: '#00ff00',
                stroke: '#000000', strokeThickness: 2
            }).setDepth(100);
            this.levelText = this.add.text(680, 16, 'LEVEL: 1', {
                fontSize: '20px', fontFamily: 'Arial, sans-serif', color: '#ffff00',
                stroke: '#000000', strokeThickness: 2
            }).setDepth(100);
            this.weaponText = this.add.text(680, 46, 'WEAPON: 1', {
                fontSize: '16px', fontFamily: 'Arial, sans-serif', color: '#ff00ff',
                stroke: '#000000', strokeThickness: 2
            }).setDepth(100);
            this.multiplierText = this.add.text(400, 80, '', {
                fontSize: '18px', fontFamily: 'Arial, sans-serif', color: '#ffff00',
                stroke: '#000000', strokeThickness: 2
            }).setOrigin(0.5).setDepth(100);
            this.bossHealthBar = this.add.graphics().setDepth(100);
            this.bossHealthBar.setVisible(false);
            this.pauseOverlay = this.add.container(400, 300).setDepth(200);
            const pauseBg = this.add.rectangle(0, 0, 300, 150, 0x000000, 0.8);
            const pauseText = this.add.text(0, -20, 'PAUSED', {
                fontSize: '48px', fontFamily: 'Arial Black, sans-serif', color: '#ffffff'
            }).setOrigin(0.5);
            const resumeText = this.add.text(0, 40, 'Press P to Resume', {
                fontSize: '20px', fontFamily: 'Arial, sans-serif', color: '#aaaaaa'
            }).setOrigin(0.5);
            this.pauseOverlay.add([pauseBg, pauseText, resumeText]);
            this.pauseOverlay.setVisible(false);
        }

        update(time: number, delta: number): void {
            if (this.isPaused) return;
            this.starLayers.forEach(star => {
                star.y += star.speed * delta / 1000;
                if (star.y > 620) {
                    star.y = -20;
                    star.x = Phaser.Math.Between(0, 800);
                }
            });
            this.handlePlayerMovement();
            if (this.spaceKey.isDown && time > this.lastFired) {
                this.fireBullet();
                this.lastFired = time + this.fireRate;
            }
            if (this.shieldActive) {
                this.playerShield.setPosition(this.player.x, this.player.y);
            }
            this.updateBossHealthBar();
            this.cleanupOffscreen();
        }

        private handlePlayerMovement(): void {
            const speed = 300;
            if (this.cursors.left.isDown || this.wasd.left.isDown) {
                this.player.setVelocityX(-speed);
            } else if (this.cursors.right.isDown || this.wasd.right.isDown) {
                this.player.setVelocityX(speed);
            } else {
                this.player.setVelocityX(0);
            }
            if (this.cursors.up.isDown || this.wasd.up.isDown) {
                this.player.setVelocityY(-speed);
            } else if (this.cursors.down.isDown || this.wasd.down.isDown) {
                this.player.setVelocityY(speed);
            } else {
                this.player.setVelocityY(0);
            }
        }

        private fireBullet(): void {
            const tex = this.weaponLevel >= 3 ? 'playerBulletPowered' : 'playerBullet';
            const spd = -500;
            if (this.weaponLevel === 1) {
                this.createBullet(this.player.x, this.player.y - 20, tex, spd);
            } else if (this.weaponLevel === 2) {
                this.createBullet(this.player.x - 10, this.player.y - 15, tex, spd);
                this.createBullet(this.player.x + 10, this.player.y - 15, tex, spd);
            } else {
                this.createBullet(this.player.x, this.player.y - 20, tex, spd);
                this.createBullet(this.player.x - 15, this.player.y - 10, tex, spd, -50);
                this.createBullet(this.player.x + 15, this.player.y - 10, tex, spd, 50);
            }
        }

        private createBullet(x: number, y: number, texture: string, speedY: number, speedX: number = 0): void {
            const bullet = this.playerBullets.get(x, y, texture) as Phaser.Physics.Arcade.Image;
            if (bullet) {
                bullet.setActive(true);
                bullet.setVisible(true);
                bullet.setVelocity(speedX, speedY);
            }
        }

        private spawnEnemy(): void {
            if (this.isPaused) return;
            if (this.enemiesKilled >= this.enemiesForBoss && !this.bossSpawned) {
                this.spawnBoss();
                return;
            }
            const x = Phaser.Math.Between(50, 750);
            const types = ['enemy1', 'enemy2', 'enemy3'];
            const weights = this.level >= 3 ? [0.35, 0.3, 0.35] : [0.5, 0.3, 0.2];
            const type = this.weightedRandom(types, weights);
            const enemy = this.physics.add.image(x, -50, type) as EnemySprite;
            enemy.enemyType = type;
            enemy.setData('health', this.getEnemyHealth(type));
            enemy.setData('points', this.getEnemyPoints(type));
            this.enemies.add(enemy);
            this.setEnemyBehavior(enemy, type);
        }

        private getEnemyHealth(type: string): number {
            const h: { [key: string]: number } = { enemy1: 1, enemy2: 1, enemy3: 3 };
            return h[type] + Math.floor(this.level / 2);
        }

        private getEnemyPoints(type: string): number {
            const p: { [key: string]: number } = { enemy1: 100, enemy2: 150, enemy3: 300 };
            return p[type];
        }

        private setEnemyBehavior(enemy: Phaser.Physics.Arcade.Image, type: string): void {
            const baseSpeed = 100 + (this.level * 10);
            if (type === 'enemy1') {
                enemy.setVelocityY(baseSpeed);
                this.time.addEvent({ delay: Phaser.Math.Between(1000, 2000), callback: () => this.enemyFire(enemy), loop: false });
            } else if (type === 'enemy2') {
                enemy.setVelocityY(baseSpeed * 1.5);
                this.tweens.add({ targets: enemy, x: enemy.x + 100, duration: 500, yoyo: true, repeat: -1, ease: 'Sine.easeInOut' });
            } else {
                enemy.setVelocityY(baseSpeed * 0.5);
                this.time.addEvent({ delay: 800, callback: () => this.enemyFire(enemy), loop: true });
            }
        }

        private enemyFire(enemy: Phaser.Physics.Arcade.Image): void {
            if (!enemy.active || this.isPaused) return;
            const bullet = this.enemyBullets.get(enemy.x, enemy.y + 20, 'enemyBullet') as Phaser.Physics.Arcade.Image;
            if (bullet) {
                bullet.setActive(true);
                bullet.setVisible(true);
                bullet.setVelocityY(200 + (this.level * 20));
            }
        }

        private spawnBoss(): void {
            this.bossSpawned = true;
            const warning = this.add.text(400, 200, 'WARNING: BOSS APPROACHING', {
                fontSize: '32px', fontFamily: 'Arial Black, sans-serif', color: '#ff0000'
            }).setOrigin(0.5).setDepth(50);
            this.tweens.add({ targets: warning, alpha: 0, duration: 500, yoyo: true, repeat: 3, onComplete: () => warning.destroy() });
            this.time.delayedCall(2000, () => {
                const boss = this.physics.add.image(400, -100, 'boss');
                boss.setData('health', 50 + (this.level * 10));
                boss.setData('maxHealth', 50 + (this.level * 10));
                boss.setData('points', 5000);
                this.bosses.add(boss);
                this.tweens.add({ targets: boss, y: 100, duration: 2000, ease: 'Power2', onComplete: () => this.startBossBehavior(boss) });
            });
        }

        private startBossBehavior(boss: Phaser.Physics.Arcade.Image): void {
            this.tweens.add({ targets: boss, x: { from: 150, to: 650 }, duration: 3000, yoyo: true, repeat: -1, ease: 'Sine.easeInOut' });
            this.time.addEvent({ delay: 500, callback: () => this.bossFire(boss), loop: true });
        }

        private bossFire(boss: Phaser.Physics.Arcade.Image): void {
            if (!boss.active || this.isPaused) return;
            for (let angle = -30; angle <= 30; angle += 15) {
                const bullet = this.enemyBullets.get(boss.x, boss.y + 40, 'bossBullet') as Phaser.Physics.Arcade.Image;
                if (bullet) {
                    bullet.setActive(true);
                    bullet.setVisible(true);
                    const rad = Phaser.Math.DegToRad(90 + angle);
                    bullet.setVelocity(Math.cos(rad) * 200, Math.sin(rad) * 200);
                }
            }
        }

        private updateBossHealthBar(): void {
            this.bossHealthBar.clear();
            const boss = this.bosses.getFirstAlive() as Phaser.Physics.Arcade.Image | null;
            if (boss) {
                this.bossHealthBar.setVisible(true);
                this.bossHealthBar.fillStyle(0x333333, 1);
                this.bossHealthBar.fillRect(200, 10, 400, 20);
                const pct = boss.getData('health') / boss.getData('maxHealth');
                const clr = pct > 0.5 ? 0x00ff00 : (pct > 0.25 ? 0xffff00 : 0xff0000);
                this.bossHealthBar.fillStyle(clr, 1);
                this.bossHealthBar.fillRect(200, 10, 400 * pct, 20);
                this.bossHealthBar.lineStyle(2, 0xffffff, 1);
                this.bossHealthBar.strokeRect(200, 10, 400, 20);
            } else {
                this.bossHealthBar.setVisible(false);
            }
        }

        private bulletHitEnemy(bullet: Phaser.Physics.Arcade.Image, enemy: Phaser.Physics.Arcade.Image): void {
            bullet.setActive(false);
            bullet.setVisible(false);
            const h = enemy.getData('health') - 1;
            enemy.setData('health', h);
            this.tweens.add({ targets: enemy, alpha: 0.5, duration: 50, yoyo: true });
            if (h <= 0) this.destroyEnemy(enemy);
        }

        private bulletHitBoss(bullet: Phaser.Physics.Arcade.Image, boss: Phaser.Physics.Arcade.Image): void {
            bullet.setActive(false);
            bullet.setVisible(false);
            const h = boss.getData('health') - 1;
            boss.setData('health', h);
            this.tweens.add({ targets: boss, alpha: 0.5, duration: 50, yoyo: true });
            if (h <= 0) this.destroyBoss(boss);
        }

        private destroyEnemy(enemy: Phaser.Physics.Arcade.Image): void {
            this.explosionEmitter.setPosition(enemy.x, enemy.y);
            this.explosionEmitter.setParticleTint([0xff6600, 0xff9900, 0xffcc00]);
            this.explosionEmitter.explode(20);
            const exp = this.add.image(enemy.x, enemy.y, 'explosion');
            this.tweens.add({ targets: exp, scale: 1.5, alpha: 0, duration: 300, onComplete: () => exp.destroy() });
            const pts = enemy.getData('points') * this.scoreMultiplier;
            this.addScore(pts);
            if (Phaser.Math.Between(1, 100) <= 15) this.spawnPowerup(enemy.x, enemy.y);
            this.enemiesKilled++;
            enemy.destroy();
        }

        private destroyBoss(boss: Phaser.Physics.Arcade.Image): void {
            for (let i = 0; i < 5; i++) {
                this.time.delayedCall(i * 200, () => {
                    if (!boss.active) return;
                    const ox = Phaser.Math.Between(-40, 40);
                    const oy = Phaser.Math.Between(-40, 40);
                    this.explosionEmitter.setPosition(boss.x + ox, boss.y + oy);
                    this.explosionEmitter.explode(30);
                });
            }
            this.time.delayedCall(1000, () => {
                const pts = boss.getData('points') * this.scoreMultiplier;
                this.addScore(pts);
                for (let i = 0; i < 3; i++) {
                    this.spawnPowerup(boss.x + Phaser.Math.Between(-50, 50), boss.y + Phaser.Math.Between(-30, 30));
                }
                boss.destroy();
                this.bossSpawned = false;
                this.enemiesKilled = 0;
                this.level++;
                this.levelText.setText('LEVEL: ' + this.level);
                this.enemiesForBoss = 20 + (this.level * 5);
            });
        }

        private playerHit(player: Phaser.Physics.Arcade.Image, bullet: Phaser.Physics.Arcade.Image): void {
            if (this.shieldActive) {
                bullet.setActive(false);
                bullet.setVisible(false);
                return;
            }
            bullet.setActive(false);
            bullet.setVisible(false);
            this.damagePlayer();
        }

        private enemyCollidePlayer(player: Phaser.Physics.Arcade.Image, enemy: Phaser.Physics.Arcade.Image): void {
            if (this.shieldActive) {
                this.destroyEnemy(enemy);
                return;
            }
            this.destroyEnemy(enemy);
            this.damagePlayer();
        }

        private damagePlayer(): void {
            this.lives--;
            this.livesText.setText('LIVES: ' + this.lives);
            this.livesText.setColor(this.lives <= 1 ? '#ff0000' : '#00ff00');
            this.tweens.add({ targets: this.player, alpha: 0.3, duration: 100, yoyo: true, repeat: 5 });
            this.cameras.main.shake(200, 0.01);
            if (this.lives <= 0) this.gameOver();
        }

        private spawnPowerup(x: number, y: number): void {
            const types = ['powerupHealth', 'powerupWeapon', 'powerupShield', 'powerupScore'];
            const type = Phaser.Utils.Array.GetRandom(types);
            const pu = this.physics.add.image(x, y, type);
            pu.setData('type', type);
            pu.setVelocityY(100);
            this.powerups.add(pu);
            this.tweens.add({ targets: pu, x: pu.x + 20, duration: 500, yoyo: true, repeat: -1, ease: 'Sine.easeInOut' });
        }

        private collectPowerup(player: Phaser.Physics.Arcade.Image, powerup: Phaser.Physics.Arcade.Image): void {
            const type = powerup.getData('type') as string;
            switch (type) {
                case 'powerupHealth':
                    this.lives = Math.min(this.lives + 1, 5);
                    this.livesText.setText('LIVES: ' + this.lives);
                    this.showPowerupText('+1 LIFE', '#00ff00');
                    break;
                case 'powerupWeapon':
                    this.weaponLevel = Math.min(this.weaponLevel + 1, 3);
                    this.weaponText.setText('WEAPON: ' + this.weaponLevel);
                    this.showPowerupText('WEAPON UP!', '#ff00ff');
                    break;
                case 'powerupShield':
                    this.activateShield();
                    this.showPowerupText('SHIELD!', '#00ffff');
                    break;
                case 'powerupScore':
                    this.scoreMultiplier = 2;
                    this.multiplierText.setText('2X SCORE!');
                    this.showPowerupText('2X MULTIPLIER!', '#ffff00');
                    this.time.delayedCall(10000, () => {
                        this.scoreMultiplier = 1;
                        this.multiplierText.setText('');
                    });
                    break;
            }
            powerup.destroy();
        }

        private activateShield(): void {
            this.shieldActive = true;
            this.playerShield.setVisible(true);
            this.playerShield.setAlpha(0.7);
            this.time.delayedCall(8000, () => {
                this.tweens.add({
                    targets: this.playerShield, alpha: 0.2, duration: 200, yoyo: true, repeat: 5,
                    onComplete: () => {
                        this.shieldActive = false;
                        this.playerShield.setVisible(false);
                    }
                });
            });
        }

        private showPowerupText(text: string, color: string): void {
            const pt = this.add.text(this.player.x, this.player.y - 50, text, {
                fontSize: '20px', fontFamily: 'Arial Black, sans-serif', color: color
            }).setOrigin(0.5).setDepth(50);
            this.tweens.add({ targets: pt, y: pt.y - 30, alpha: 0, duration: 1000, onComplete: () => pt.destroy() });
        }

        private addScore(points: number): void {
            this.score += points;
            this.scoreText.setText('SCORE: ' + this.score);
            const popup = this.add.text(this.player.x + 30, this.player.y, '+' + points, {
                fontSize: '16px', fontFamily: 'Arial, sans-serif', color: '#ffff00'
            }).setDepth(50);
            this.tweens.add({ targets: popup, y: popup.y - 30, alpha: 0, duration: 500, onComplete: () => popup.destroy() });
        }

        private increaseDifficulty(): void {
            if (this.spawnTimer.delay > 500) {
                const newDelay = this.spawnTimer.delay - 200;
                this.spawnTimer.destroy();
                this.spawnTimer = this.time.addEvent({
                    delay: newDelay, callback: this.spawnEnemy, callbackScope: this, loop: true
                });
            }
        }

        private cleanupOffscreen(): void {
            this.playerBullets.children.each((b) => {
                const bullet = b as Phaser.Physics.Arcade.Image;
                if (bullet.active && (bullet.y < -20 || bullet.y > 620)) {
                    bullet.setActive(false);
                    bullet.setVisible(false);
                }
                return true;
            });
            this.enemyBullets.children.each((b) => {
                const bullet = b as Phaser.Physics.Arcade.Image;
                if (bullet.active && (bullet.y < -20 || bullet.y > 620)) {
                    bullet.setActive(false);
                    bullet.setVisible(false);
                }
                return true;
            });
            this.enemies.children.each((e) => {
                const enemy = e as Phaser.Physics.Arcade.Image;
                if (enemy.y > 650) enemy.destroy();
                return true;
            });
            this.powerups.children.each((p) => {
                const powerup = p as Phaser.Physics.Arcade.Image;
                if (powerup.y > 650) powerup.destroy();
                return true;
            });
        }

        private togglePause(): void {
            this.isPaused = !this.isPaused;
            this.pauseOverlay.setVisible(this.isPaused);
            if (this.isPaused) {
                this.physics.pause();
                this.tweens.pauseAll();
            } else {
                this.physics.resume();
                this.tweens.resumeAll();
            }
        }

        private weightedRandom(items: string[], weights: number[]): string {
            const total = weights.reduce((s, w) => s + w, 0);
            let r = Math.random() * total;
            for (let i = 0; i < items.length; i++) {
                if (r < weights[i]) return items[i];
                r -= weights[i];
            }
            return items[items.length - 1];
        }

        private gameOver(): void {
            const hs = localStorage.getItem('spaceShooterHighScore') || '0';
            if (this.score > parseInt(hs)) localStorage.setItem('spaceShooterHighScore', this.score.toString());
            this.spawnTimer.destroy();
            this.explosionEmitter.setPosition(this.player.x, this.player.y);
            this.explosionEmitter.explode(50);
            this.player.setVisible(false);
            this.engineTrail.stop();
            this.time.delayedCall(1500, () => this.scene.start('GameOverScene', { score: this.score, level: this.level }));
        }
    }

    class GameOverScene extends Phaser.Scene {
        private finalScore!: number;
        private finalLevel!: number;

        constructor() {
            super({ key: 'GameOverScene' });
        }

        init(data: GameOverData): void {
            this.finalScore = data.score || 0;
            this.finalLevel = data.level || 1;
        }

        create(): void {
            this.add.rectangle(400, 300, 800, 600, 0x000011);
            this.add.text(400, 150, 'GAME OVER', {
                fontSize: '72px', fontFamily: 'Arial Black, sans-serif',
                color: '#ff0000', stroke: '#660000', strokeThickness: 8
            }).setOrigin(0.5);
            this.add.text(400, 260, 'FINAL SCORE: ' + this.finalScore, {
                fontSize: '36px', fontFamily: 'Arial, sans-serif', color: '#ffffff'
            }).setOrigin(0.5);
            this.add.text(400, 310, 'LEVEL REACHED: ' + this.finalLevel, {
                fontSize: '28px', fontFamily: 'Arial, sans-serif', color: '#ffff00'
            }).setOrigin(0.5);
            const hs = localStorage.getItem('spaceShooterHighScore') || '0';
            const isNew = this.finalScore >= parseInt(hs) && this.finalScore > 0;
            if (isNew) {
                const newHsText = this.add.text(400, 360, 'NEW HIGH SCORE!', {
                    fontSize: '32px', fontFamily: 'Arial Black, sans-serif', color: '#00ff00'
                }).setOrigin(0.5);
                this.tweens.add({ targets: newHsText, scale: 1.2, duration: 500, yoyo: true, repeat: -1 });
            } else {
                this.add.text(400, 360, 'HIGH SCORE: ' + hs, {
                    fontSize: '24px', fontFamily: 'Arial, sans-serif', color: '#aaaaaa'
                }).setOrigin(0.5);
            }
            this.createButton(400, 450, 'PLAY AGAIN', () => this.scene.start('GameScene'));
            this.createButton(400, 520, 'MAIN MENU', () => this.scene.start('MainMenuScene'));
        }

        private createButton(x: number, y: number, text: string, callback: () => void): Phaser.GameObjects.Container {
            const btn = this.add.container(x, y);
            const bg = this.add.rectangle(0, 0, 220, 50, 0x0044aa, 0.8).setStrokeStyle(2, 0x00ffff);
            const lbl = this.add.text(0, 0, text, { fontSize: '24px', fontFamily: 'Arial, sans-serif', color: '#ffffff' }).setOrigin(0.5);
            btn.add([bg, lbl]);
            btn.setSize(220, 50);
            btn.setInteractive({ useHandCursor: true });
            btn.on('pointerover', () => bg.setFillStyle(0x0066cc, 1));
            btn.on('pointerout', () => bg.setFillStyle(0x0044aa, 0.8));
            btn.on('pointerdown', callback);
            return btn;
        }
    }

    let spaceShooterInstance: SpaceShooterGame | null = null;
    window.SpaceShooter = {
        init: function (containerId: string): Phaser.Game {
            if (spaceShooterInstance) spaceShooterInstance.destroy();
            spaceShooterInstance = new SpaceShooterGame(containerId);
            return spaceShooterInstance.init();
        },
        destroy: function (): void {
            if (spaceShooterInstance) {
                spaceShooterInstance.destroy();
                spaceShooterInstance = null;
            }
        }
    };
}
