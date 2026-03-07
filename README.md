<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>8/3 Rực Rỡ Trên Nền Đêm</title>
    <style>
        body, html {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            overflow: hidden;
            /* CHUYỂN NỀN SANG MÀU ĐEN */
            background-color: #000000; 
            font-family: 'Arial', sans-serif;
        }
        canvas {
            display: block;
            width: 100%;
            height: 100%;
        }
    </style>
</head>
<body>

<canvas id="canvas"></canvas>

<script>
    const canvas = document.getElementById('canvas');
    // willReadFrequently giúp tối ưu hóa việc đọc dữ liệu ảnh để tạo hạt
    const ctx = canvas.getContext('2d', { willReadFrequently: true });

    let width = canvas.width = window.innerWidth;
    let height = canvas.height = window.innerHeight;

    let textParticles = [];
    let hearts = [];
    let stars = [];
    let fireworks = [];
    let fireworkParticles = [];

    // --- CẤU HÌNH TEXT ---
    const text1 = "Chúc bạn";
    const text2 = "8/3 vui vẻ!";

    function initText() {
        textParticles = [];
        const tempCanvas = document.createElement('canvas');
        const tempCtx = tempCanvas.getContext('2d', { willReadFrequently: true });
        tempCanvas.width = width;
        tempCanvas.height = height;

        // Cấu hình Font chữ to và đậm
        let fontSize = Math.min(width / 5, 180); 
        tempCtx.font = `bold ${fontSize}px Arial`;
        tempCtx.fillStyle = "white";
        tempCtx.textAlign = "center";
        tempCtx.textBaseline = "middle";

        tempCtx.fillText(text1, width / 2, height / 2 - fontSize * 0.6);
        tempCtx.fillText(text2, width / 2, height / 2 + fontSize * 0.6);

        const textData = tempCtx.getImageData(0, 0, width, height).data;
        let validPixels = [];

        // Thu thập tất cả các điểm ảnh của chữ (bước quét nhỏ để lấy nhiều điểm)
        for (let y = 0; y < height; y += 1) { // Quét chi tiết từng pixel y
            for (let x = 0; x < width; x += 1) { // Quét chi tiết từng pixel x
                const alpha = textData[(y * width + x) * 4 + 3];
                if (alpha > 128) {
                    validPixels.push({x, y});
                }
            }
        }

        // --- TĂNG SỐ HẠT LÊN RẤT NHIỀU (khoảng 10.000 hạt) ---
        let targetCount = 5000; 
        if (validPixels.length > 0) {
            let step = Math.max(1, Math.floor(validPixels.length / targetCount));
            for (let i = 0; i < validPixels.length; i += step) {
                if (textParticles.length < targetCount) {
                    textParticles.push(new TextParticle(validPixels[i].x, validPixels[i].y));
                }
            }
        }
    }

    // --- LỚP HẠT TEXT (SIÊU NHỎ, LẤP LÁNH) ---
    class TextParticle {
        constructor(targetX, targetY) {
            this.tx = targetX; // Vị trí đích
            this.ty = targetY;
            // Xuất phát ngẫu nhiên ngoài màn hình
            this.x = Math.random() * width;
            this.y = Math.random() < 0.5 ? -100 : height + 100;
            
            // Hạt siêu nhỏ để tạo độ mịn cho chữ
            this.size = Math.random() * 1.0 + 0.5; 
            // Màu sắc rực rỡ nổi bật trên nền đen
            this.color = `hsl(${Math.random() * 360}, 100%, 75%)`; 
            // Tốc độ gom lại
            this.speed = Math.random() * 0.02 + 0.005; 
            this.distSq = Math.random() * 2000 + 500; // Độ phân tán ban đầu
        }
        update() {
            // Hiệu ứng di chuyển gom vào
            this.x += (this.tx - this.x) * this.speed;
            this.y += (this.ty - this.y) * this.speed;
        }
        draw() {
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fillStyle = this.color;
            // Hiệu ứng phát sáng mạnh trên nền đen
            ctx.shadowBlur = 4; 
            ctx.shadowColor = this.color;
            ctx.fill();
            ctx.shadowBlur = 0; // Reset để không ảnh hưởng đối tượng khác
        }
    }

    // --- LỚP TRÁI TIM (ĐỎ RỰC, PHÁT SÁNG) ---
    class Heart {
        constructor() {
            this.reset();
            this.life = Math.random() * this.maxLife; // Chênh lệch thời gian sinh ra
        }
        reset() {
            this.x = Math.random() * width;
            this.y = height + 100 + Math.random() * 500; // Sinh ra từ dưới sâu
            this.size = Math.random() * 10 + 5; // Kích thước
            this.maxLife = Math.random() * 200 + 100; // Sống lâu hơn
            this.life = 0;
            // Màu đỏ tươi rực rỡ
            this.color = `hsl(350, 100%, 60%)`; 
            this.speedY = Math.random() * 1 + 0.5; // Bay lên
        }
        update() {
            this.life++;
            this.y -= this.speedY;
            if (this.life >= this.maxLife || this.y < -100) {
                this.reset();
            }
        }
        draw() {
            // Hiệu ứng tỏ mờ (sin wave)
            let opacity = Math.sin((this.life / this.maxLife) * Math.PI);
            ctx.save();
            ctx.globalAlpha = opacity;
            ctx.shadowBlur = 15; // Phát sáng mạnh
            ctx.shadowColor = this.color;
            ctx.fillStyle = this.color;
            
            // Vẽ hình trái tim
            ctx.translate(this.x, this.y);
            ctx.beginPath();
            ctx.moveTo(0, -this.size * 0.3);
            ctx.bezierCurveTo(0, -this.size, -this.size, -this.size, -this.size, -this.size * 0.3);
            ctx.bezierCurveTo(-this.size, this.size * 0.2, 0, this.size * 0.6, 0, this.size);
            ctx.bezierCurveTo(0, this.size * 0.6, this.size, this.size * 0.2, this.size, -this.size * 0.3);
            ctx.bezierCurveTo(this.size, -this.size, 0, -this.size, 0, -this.size * 0.3);
            ctx.fill();
            ctx.restore();
        }
    }

    // --- LỚP NGÔI SAO NỀN (LẤP LÁNH) ---
    class Star {
        constructor() {
            this.x = Math.random() * width;
            this.y = Math.random() * height;
            this.size = Math.random() * 1.5 + 0.5;
            this.opacity = Math.random();
            this.speed = Math.random() * 0.01 + 0.005;
        }
        update() {
            this.opacity += this.speed;
            if (this.opacity > 1 || this.opacity < 0) this.speed = -this.speed;
        }
        draw() {
            ctx.save();
            ctx.globalAlpha = Math.abs(this.opacity);
            ctx.fillStyle = "white";
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
    }

    // --- LỚP PHÁO HOA (MẠNH MẼ, NHIỀU MÀU) ---
    class Firework {
        constructor() {
            this.x = Math.random() * width;
            this.y = height;
            this.targetY = height * 0.1 + Math.random() * height * 0.4; // Nổ ở 1/2 trên màn hình
            this.speed = Math.random() * 3 + 5;
            // Màu pháo hoa neon cực rực rỡ
            let colors = ['#ff0055', '#00ff55', '#0055ff', '#ffff00', '#ff00ff', '#00ffff'];
            this.color = colors[Math.floor(Math.random() * colors.length)];
            this.dead = false;
        }
        update() {
            this.y -= this.speed;
            this.speed *= 0.99; // Chậm dần khi lên cao
            if (this.speed < 1 || this.y <= this.targetY) {
                this.dead = true;
                this.explode();
            }
        }
        draw() {
            ctx.beginPath();
            ctx.arc(this.x, this.y, 3, 0, Math.PI * 2);
            ctx.fillStyle = this.color;
            ctx.shadowBlur = 10;
            ctx.shadowColor = this.color;
            ctx.fill();
            ctx.shadowBlur = 0;
        }
        explode() {
            // Tăng số lượng hạt tàn pháo lên 100 hạt mỗi quả
            for (let i = 0; i < 100; i++) {
                fireworkParticles.push(new FireworkParticle(this.x, this.y, this.color));
            }
        }
    }

    class FireworkParticle {
        constructor(x, y, color) {
            this.x = x;
            this.y = y;
            this.color = color;
            let angle = Math.random() * Math.PI * 2;
            let speed = Math.random() * 6 + 2; // Nổ to hơn
            this.vx = Math.cos(angle) * speed;
            this.vy = Math.sin(angle) * speed;
            this.life = 1;
            this.decay = Math.random() * 0.015 + 0.01; // Tàn lâu hơn
        }
        update() {
            this.vx *= 0.95; // Sức cản không khí
            this.vy *= 0.95;
            this.vy += 0.08; // Trọng lực
            this.x += this.vx;
            this.y += this.vy;
            this.life -= this.decay;
        }
        draw() {
            ctx.save();
            ctx.globalAlpha = this.life;
            ctx.fillStyle = this.color;
            ctx.shadowBlur = 8; // Tàn pháo phát sáng
            ctx.shadowColor = this.color;
            ctx.beginPath();
            ctx.arc(this.x, this.y, 2, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
    }

    // --- KHỞI TẠO VÀ VÒNG LẶP ---
    function init() {
        initText();
        
        // Tăng số lượng trái tim (200 trái tim)
        hearts = [];
        for (let i = 0; i < 200; i++) hearts.push(new Heart());

        // Tăng số lượng sao nền (150 ngôi sao)
        stars = [];
        for (let i = 0; i < 150; i++) stars.push(new Star());
        
        fireworks = [];
        fireworkParticles = [];
    }

    function animate() {
        // Vẽ nền đen mờ nhẹ để tạo hiệu ứng đuôi ánh sáng (motion blur)
        ctx.fillStyle = 'rgba(0, 0, 0, 0.1)';
        ctx.fillRect(0, 0, width, height);

        // Vẽ sao nền
        stars.forEach(star => { star.update(); star.draw(); });
        
        // Vẽ trái tim
        hearts.forEach(heart => { heart.update(); heart.draw(); });
        
        // Vẽ và quản lý pháo hoa
        if (Math.random() < 0.04) { // Tăng tỷ lệ xuất hiện pháo hoa
            fireworks.push(new Firework());
        }
        for (let i = fireworks.length - 1; i >= 0; i--) {
            fireworks[i].update();
            fireworks[i].draw();
            if (fireworks[i].dead) fireworks.splice(i, 1);
        }
        for (let i = fireworkParticles.length - 1; i >= 0; i--) {
            fireworkParticles[i].update();
            fireworkParticles[i].draw();
            if (fireworkParticles[i].life <= 0) fireworkParticles.splice(i, 1);
        }

        // Vẽ chữ (sau cùng để nằm trên cùng)
        textParticles.forEach(p => { p.update(); p.draw(); });

        requestAnimationFrame(animate);
    }

    // Xử lý khi đổi kích thước màn hình
    window.addEventListener('resize', () => {
        width = canvas.width = window.innerWidth;
        height = canvas.height = window.innerHeight;
        init();
    });

    init();
    animate();

</script>

</body>
</html>
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>8/3 Rực Rỡ Trên Nền Đêm</title>
    <style>
        body, html {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            overflow: hidden;
            /* CHUYỂN NỀN SANG MÀU ĐEN */
            background-color: #000000; 
            font-family: 'Arial', sans-serif;
        }
        canvas {
            display: block;
            width: 100%;
            height: 100%;
        }
    </style>
</head>
<body>

<canvas id="canvas"></canvas>

<script>
    const canvas = document.getElementById('canvas');
    // willReadFrequently giúp tối ưu hóa việc đọc dữ liệu ảnh để tạo hạt
    const ctx = canvas.getContext('2d', { willReadFrequently: true });

    let width = canvas.width = window.innerWidth;
    let height = canvas.height = window.innerHeight;

    let textParticles = [];
    let hearts = [];
    let stars = [];
    let fireworks = [];
    let fireworkParticles = [];

    // --- CẤU HÌNH TEXT ---
    const text1 = "Chúc bạn";
    const text2 = "8/3 vui vẻ!";

    function initText() {
        textParticles = [];
        const tempCanvas = document.createElement('canvas');
        const tempCtx = tempCanvas.getContext('2d', { willReadFrequently: true });
        tempCanvas.width = width;
        tempCanvas.height = height;

        // Cấu hình Font chữ to và đậm
        let fontSize = Math.min(width / 5, 180); 
        tempCtx.font = `bold ${fontSize}px Arial`;
        tempCtx.fillStyle = "white";
        tempCtx.textAlign = "center";
        tempCtx.textBaseline = "middle";

        tempCtx.fillText(text1, width / 2, height / 2 - fontSize * 0.6);
        tempCtx.fillText(text2, width / 2, height / 2 + fontSize * 0.6);

        const textData = tempCtx.getImageData(0, 0, width, height).data;
        let validPixels = [];

        // Thu thập tất cả các điểm ảnh của chữ (bước quét nhỏ để lấy nhiều điểm)
        for (let y = 0; y < height; y += 1) { // Quét chi tiết từng pixel y
            for (let x = 0; x < width; x += 1) { // Quét chi tiết từng pixel x
                const alpha = textData[(y * width + x) * 4 + 3];
                if (alpha > 128) {
                    validPixels.push({x, y});
                }
            }
        }

        // --- TĂNG SỐ HẠT LÊN RẤT NHIỀU (khoảng 10.000 hạt) ---
        let targetCount = 5000; 
        if (validPixels.length > 0) {
            let step = Math.max(1, Math.floor(validPixels.length / targetCount));
            for (let i = 0; i < validPixels.length; i += step) {
                if (textParticles.length < targetCount) {
                    textParticles.push(new TextParticle(validPixels[i].x, validPixels[i].y));
                }
            }
        }
    }

    // --- LỚP HẠT TEXT (SIÊU NHỎ, LẤP LÁNH) ---
    class TextParticle {
        constructor(targetX, targetY) {
            this.tx = targetX; // Vị trí đích
            this.ty = targetY;
            // Xuất phát ngẫu nhiên ngoài màn hình
            this.x = Math.random() * width;
            this.y = Math.random() < 0.5 ? -100 : height + 100;
            
            // Hạt siêu nhỏ để tạo độ mịn cho chữ
            this.size = Math.random() * 1.0 + 0.5; 
            // Màu sắc rực rỡ nổi bật trên nền đen
            this.color = `hsl(${Math.random() * 360}, 100%, 75%)`; 
            // Tốc độ gom lại
            this.speed = Math.random() * 0.02 + 0.005; 
            this.distSq = Math.random() * 2000 + 500; // Độ phân tán ban đầu
        }
        update() {
            // Hiệu ứng di chuyển gom vào
            this.x += (this.tx - this.x) * this.speed;
            this.y += (this.ty - this.y) * this.speed;
        }
        draw() {
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fillStyle = this.color;
            // Hiệu ứng phát sáng mạnh trên nền đen
            ctx.shadowBlur = 4; 
            ctx.shadowColor = this.color;
            ctx.fill();
            ctx.shadowBlur = 0; // Reset để không ảnh hưởng đối tượng khác
        }
    }

    // --- LỚP TRÁI TIM (ĐỎ RỰC, PHÁT SÁNG) ---
    class Heart {
        constructor() {
            this.reset();
            this.life = Math.random() * this.maxLife; // Chênh lệch thời gian sinh ra
        }
        reset() {
            this.x = Math.random() * width;
            this.y = height + 100 + Math.random() * 500; // Sinh ra từ dưới sâu
            this.size = Math.random() * 10 + 5; // Kích thước
            this.maxLife = Math.random() * 200 + 100; // Sống lâu hơn
            this.life = 0;
            // Màu đỏ tươi rực rỡ
            this.color = `hsl(350, 100%, 60%)`; 
            this.speedY = Math.random() * 1 + 0.5; // Bay lên
        }
        update() {
            this.life++;
            this.y -= this.speedY;
            if (this.life >= this.maxLife || this.y < -100) {
                this.reset();
            }
        }
        draw() {
            // Hiệu ứng tỏ mờ (sin wave)
            let opacity = Math.sin((this.life / this.maxLife) * Math.PI);
            ctx.save();
            ctx.globalAlpha = opacity;
            ctx.shadowBlur = 15; // Phát sáng mạnh
            ctx.shadowColor = this.color;
            ctx.fillStyle = this.color;
            
            // Vẽ hình trái tim
            ctx.translate(this.x, this.y);
            ctx.beginPath();
            ctx.moveTo(0, -this.size * 0.3);
            ctx.bezierCurveTo(0, -this.size, -this.size, -this.size, -this.size, -this.size * 0.3);
            ctx.bezierCurveTo(-this.size, this.size * 0.2, 0, this.size * 0.6, 0, this.size);
            ctx.bezierCurveTo(0, this.size * 0.6, this.size, this.size * 0.2, this.size, -this.size * 0.3);
            ctx.bezierCurveTo(this.size, -this.size, 0, -this.size, 0, -this.size * 0.3);
            ctx.fill();
            ctx.restore();
        }
    }

    // --- LỚP NGÔI SAO NỀN (LẤP LÁNH) ---
    class Star {
        constructor() {
            this.x = Math.random() * width;
            this.y = Math.random() * height;
            this.size = Math.random() * 1.5 + 0.5;
            this.opacity = Math.random();
            this.speed = Math.random() * 0.01 + 0.005;
        }
        update() {
            this.opacity += this.speed;
            if (this.opacity > 1 || this.opacity < 0) this.speed = -this.speed;
        }
        draw() {
            ctx.save();
            ctx.globalAlpha = Math.abs(this.opacity);
            ctx.fillStyle = "white";
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
    }

    // --- LỚP PHÁO HOA (MẠNH MẼ, NHIỀU MÀU) ---
    class Firework {
        constructor() {
            this.x = Math.random() * width;
            this.y = height;
            this.targetY = height * 0.1 + Math.random() * height * 0.4; // Nổ ở 1/2 trên màn hình
            this.speed = Math.random() * 3 + 5;
            // Màu pháo hoa neon cực rực rỡ
            let colors = ['#ff0055', '#00ff55', '#0055ff', '#ffff00', '#ff00ff', '#00ffff'];
            this.color = colors[Math.floor(Math.random() * colors.length)];
            this.dead = false;
        }
        update() {
            this.y -= this.speed;
            this.speed *= 0.99; // Chậm dần khi lên cao
            if (this.speed < 1 || this.y <= this.targetY) {
                this.dead = true;
                this.explode();
            }
        }
        draw() {
            ctx.beginPath();
            ctx.arc(this.x, this.y, 3, 0, Math.PI * 2);
            ctx.fillStyle = this.color;
            ctx.shadowBlur = 10;
            ctx.shadowColor = this.color;
            ctx.fill();
            ctx.shadowBlur = 0;
        }
        explode() {
            // Tăng số lượng hạt tàn pháo lên 100 hạt mỗi quả
            for (let i = 0; i < 100; i++) {
                fireworkParticles.push(new FireworkParticle(this.x, this.y, this.color));
            }
        }
    }

    class FireworkParticle {
        constructor(x, y, color) {
            this.x = x;
            this.y = y;
            this.color = color;
            let angle = Math.random() * Math.PI * 2;
            let speed = Math.random() * 6 + 2; // Nổ to hơn
            this.vx = Math.cos(angle) * speed;
            this.vy = Math.sin(angle) * speed;
            this.life = 1;
            this.decay = Math.random() * 0.015 + 0.01; // Tàn lâu hơn
        }
        update() {
            this.vx *= 0.95; // Sức cản không khí
            this.vy *= 0.95;
            this.vy += 0.08; // Trọng lực
            this.x += this.vx;
            this.y += this.vy;
            this.life -= this.decay;
        }
        draw() {
            ctx.save();
            ctx.globalAlpha = this.life;
            ctx.fillStyle = this.color;
            ctx.shadowBlur = 8; // Tàn pháo phát sáng
            ctx.shadowColor = this.color;
            ctx.beginPath();
            ctx.arc(this.x, this.y, 2, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
    }

    // --- KHỞI TẠO VÀ VÒNG LẶP ---
    function init() {
        initText();
        
        // Tăng số lượng trái tim (200 trái tim)
        hearts = [];
        for (let i = 0; i < 200; i++) hearts.push(new Heart());

        // Tăng số lượng sao nền (150 ngôi sao)
        stars = [];
        for (let i = 0; i < 150; i++) stars.push(new Star());
        
        fireworks = [];
        fireworkParticles = [];
    }

    function animate() {
        // Vẽ nền đen mờ nhẹ để tạo hiệu ứng đuôi ánh sáng (motion blur)
        ctx.fillStyle = 'rgba(0, 0, 0, 0.1)';
        ctx.fillRect(0, 0, width, height);

        // Vẽ sao nền
        stars.forEach(star => { star.update(); star.draw(); });
        
        // Vẽ trái tim
        hearts.forEach(heart => { heart.update(); heart.draw(); });
        
        // Vẽ và quản lý pháo hoa
        if (Math.random() < 0.04) { // Tăng tỷ lệ xuất hiện pháo hoa
            fireworks.push(new Firework());
        }
        for (let i = fireworks.length - 1; i >= 0; i--) {
            fireworks[i].update();
            fireworks[i].draw();
            if (fireworks[i].dead) fireworks.splice(i, 1);
        }
        for (let i = fireworkParticles.length - 1; i >= 0; i--) {
            fireworkParticles[i].update();
            fireworkParticles[i].draw();
            if (fireworkParticles[i].life <= 0) fireworkParticles.splice(i, 1);
        }

        // Vẽ chữ (sau cùng để nằm trên cùng)
        textParticles.forEach(p => { p.update(); p.draw(); });

        requestAnimationFrame(animate);
    }

    // Xử lý khi đổi kích thước màn hình
    window.addEventListener('resize', () => {
        width = canvas.width = window.innerWidth;
        height = canvas.height = window.innerHeight;
        init();
    });

    init();
    animate();

</script>

</body>
</html>
