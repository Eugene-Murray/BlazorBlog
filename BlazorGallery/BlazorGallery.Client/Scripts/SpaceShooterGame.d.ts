/// <reference types="phaser" />

declare global {
    interface Window {
        SpaceShooter: {
            init: (containerId: string) => Phaser.Game;
            destroy: () => void;
        };
    }
}

export {};
