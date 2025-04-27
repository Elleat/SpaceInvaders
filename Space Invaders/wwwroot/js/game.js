const canvas = document.getElementById("gameCanvas");
const ctx = canvas.getContext("2d");

// Игрок
const player = {
    x: canvas.width / 2,
    y: canvas.height - 50,
    width: 50,
    height: 30,
    speed: 5,
    color: "#00FF00"
};

// Враги
const enemies = [];
function createEnemy() {
    enemies.push({
        x: Math.random() * (canvas.width - 30),
        y: 30,
        width: 30,
        height: 30,
        speed: 2,
        color: "#FF0000"
    });
}

// Пули
const bullets = [];
function shoot() {
    bullets.push({
        x: player.x + player.width / 2 - 2.5,
        y: player.y,
        width: 5,
        height: 10,
        speed: 7,
        color: "#FFFF00"
    });
}

// Управление
let rightPressed = false;
let leftPressed = false;
document.addEventListener("keydown", (e) => {
    if (e.key === "ArrowRight") rightPressed = true;
    if (e.key === "ArrowLeft") leftPressed = true;
    if (e.key === " ") shoot(); // Пробел для стрельбы
});
document.addEventListener("keyup", (e) => {
    if (e.key === "ArrowRight") rightPressed = false;
    if (e.key === "ArrowLeft") leftPressed = false;
});

// Игровой цикл
function gameLoop() {
    // Очистка экрана
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Движение игрока
    if (rightPressed && player.x < canvas.width - player.width) player.x += player.speed;
    if (leftPressed && player.x > 0) player.x -= player.speed;

    // Отрисовка игрока
    ctx.fillStyle = player.color;
    ctx.fillRect(player.x, player.y, player.width, player.height);

    // Отрисовка врагов
    enemies.forEach(enemy => {
        ctx.fillStyle = enemy.color;
        ctx.fillRect(enemy.x, enemy.y, enemy.width, enemy.height);
        enemy.y += enemy.speed;
    });

    // Отрисовка пуль
    bullets.forEach((bullet, index) => {
        ctx.fillStyle = bullet.color;
        ctx.fillRect(bullet.x, bullet.y, bullet.width, bullet.height);
        bullet.y -= bullet.speed;

        // Удаление пуль за пределами экрана
        if (bullet.y < 0) bullets.splice(index, 1);
    });

    // Проверка столкновений пуль с врагами
    function checkCollisions() {
        bullets.forEach((bullet, bulletIndex) => {
            enemies.forEach((enemy, enemyIndex) => {
                if (
                    bullet.x < enemy.x + enemy.width &&
                    bullet.x + bullet.width > enemy.x &&
                    bullet.y < enemy.y + enemy.height &&
                    bullet.y + bullet.height > enemy.y
                ) {
                    // Удаляем пулю и врага при столкновении
                    bullets.splice(bulletIndex, 1);
                    enemies.splice(enemyIndex, 1);
                    score += 10; // Добавляем очки
                }
            });
        });
    }

    // Игровой цикл
    function gameLoop() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Движение и отрисовка игрока, врагов, пуль (как раньше)
        // ...

        checkCollisions(); // <-- Добавляем проверку коллизий

        // Отображение счёта
        ctx.fillStyle = "#FFFFFF";
        ctx.font = "20px Arial";
        ctx.fillText(`Score: ${score}`, 10, 30);

        requestAnimationFrame(gameLoop);
    }

    let score = 0; // Добавляем переменную для счёта

    // Генерация врагов
    if (Math.random() < 0.02) createEnemy();

    requestAnimationFrame(gameLoop);
}

gameLoop();