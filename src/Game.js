import React, { useRef, useEffect, useState } from 'react';
import UNIT from './constants.js';
import Character from './components/character';
import './Game.css'

function Game() {
    const canvasRef = useRef(null);
    const [direction, setDirection] = useState('none');
    const character = Character({ canvasRef, direction });

    function handleKeyPress(event) {
        switch (event.key) {
            case 'ArrowUp':
                setDirection('up');
                break;
            case 'ArrowDown':
                setDirection('down');
                break;
            case 'ArrowLeft':
                setDirection('left');
                break;
            case 'ArrowRight':
                setDirection('right');
                break;
            default:
                setDirection('none');
                break;
        }
    }

    function animate() {
        character.drawCircle();
        character.update();
    }

    useEffect(() => {
        const canvas = canvasRef.current;
        canvas.width = 60 * UNIT;
        canvas.height = 40 * UNIT;

        window.addEventListener('keydown', handleKeyPress);

        const drawInterval = setInterval(animate, 1000 / 120);

        return () => {
            clearInterval(drawInterval); // Cleanup on unmount
            window.removeEventListener('keydown', handleKeyPress); // Cleanup event listener
        };
    }, [character]);

    return (
        <div className="container">
            <canvas ref={canvasRef} />
        </div>
    );
}

export default Game;