import React, { useEffect, useRef } from 'react';
import UNIT from '../constants.js';

function Character({ canvasRef, direction }) {
    const circleRef = useRef({
        x: 0,
        y: 0,
        radius: 1 * UNIT,
        dx: 0,
        dy: 0,
        speed: 0.2 * UNIT
    });

    useEffect(() => {
        const canvas = canvasRef.current;
        circleRef.current.x = canvas.width / 2;
        circleRef.current.y = canvas.height / 2;
    }, [canvasRef]);

    function drawCircle() {
        const canvas = canvasRef.current;
        const ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.beginPath();
        ctx.arc(circleRef.current.x, circleRef.current.y, circleRef.current.radius, 0, Math.PI * 2);
        ctx.fillStyle = 'blue';
        ctx.fill();
        ctx.closePath();
    }

    function update() {
        const canvas = canvasRef.current;

        switch (direction) {
            case 'up':
                circleRef.current.dy = -circleRef.current.speed;
                circleRef.current.dx = 0;
                break;
            case 'down':
                circleRef.current.dy = circleRef.current.speed;
                circleRef.current.dx = 0;
                break;
            case 'right':
                circleRef.current.dy = 0;
                circleRef.current.dx = circleRef.current.speed;
                break;
            case 'left':
                circleRef.current.dy = 0;
                circleRef.current.dx = -circleRef.current.speed;
                break;
            case 'none':
                circleRef.current.dx = 0;
                circleRef.current.dy = 0;
                break;
        }

        // Check for wall collisions
        if (circleRef.current.x + circleRef.current.radius + circleRef.current.dx > canvas.width || circleRef.current.x + circleRef.current.dx - circleRef.current.radius < 0) {
            // circleRef.current.dx = -circleRef.current.dx;
            circleRef.current.dx = 0;
            circleRef.current.dy = 0;
        }
        if (circleRef.current.y + circleRef.current.radius + circleRef.current.dy > canvas.height || circleRef.current.y + circleRef.current.dy - circleRef.current.radius < 0) {
            // circleRef.current.dy = -circleRef.current.dy;
            circleRef.current.dx = 0;
            circleRef.current.dy = 0;
        }
        circleRef.current.x += circleRef.current.dx;
        circleRef.current.y += circleRef.current.dy;
    }

    return { drawCircle, update };
}

export default Character;