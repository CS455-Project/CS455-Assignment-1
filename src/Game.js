import React, { useRef, useEffect, useState } from 'react';

function Game() {
    const canvasRef = useRef(null);
    const [bird, setBird] = useState({
        x: 50,
        y: 100,  // We'll update this in useEffect
        radius: 20
    });

    const draw = () => {
        const canvas = canvasRef.current;
        const ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = "green";
        ctx.beginPath();
        ctx.arc(bird.x, bird.y, bird.radius, 0, 2 * Math.PI);
        ctx.fill();
        ctx.stroke();
    };
    
    useEffect(() => {
        const canvas = canvasRef.current;
        if (canvas) {
            setBird(prevBird => ({
                ...prevBird,
                y: canvas.height / 2
            }));
        }
        
        const drawInterval = setInterval(() => {
            console.log("calling draw");
            // setBird(prevBird => ({
            //     ...prevBird,
            //     // radius : 2 + prevBird.radius,
            // y : prevBird.x + 2,
            // }));
            
            draw();
        }, 1000 / 60);

        // Cleanup function to clear the interval when component unmounts
        return () => clearInterval(drawInterval);
    }, []);

    useEffect(() => {
        draw();
    },[bird]);

    return (
        <div>
            <canvas ref={canvasRef} width={450} height={650} />
        </div>
    );
}

export default Game;