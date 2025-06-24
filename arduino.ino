#include <Servo.h>

// Declaramos los tres servos
Servo servoBase;
Servo servoBrazo1;
Servo servoBrazo2;

// Posiciones actuales
int basePos = 0;
int brazo1Pos = 0;
int brazo2Pos = 0;

// ✅ Función: Movimiento suave para 1 servo
void moverServoSuave(Servo servo, int desde, int hasta, int paso = 1, int retardo = 15) {
  if (desde < hasta) {
    for (int pos = desde; pos <= hasta; pos += paso) {
      servo.write(pos);
      delay(retardo);
    }
  } else {
    for (int pos = desde; pos >= hasta; pos -= paso) {
      servo.write(pos);
      delay(retardo);
    }
  }
}

// ✅ Función: Movimiento simultáneo de los dos brazos, brazo1 más rápido
void moverBrazosSimultaneo(int desde1, int hasta1, int desde2, int hasta2) {
  int pasos1 = abs(hasta1 - desde1);
  int pasos2 = abs(hasta2 - desde2);

  int maxPasos = max(pasos1, pasos2);

  for (int i = 0; i <= maxPasos; i++) {
    // Interpolación lineal para cada paso
    if (i <= pasos1) {
      int val1 = map(i, 0, pasos1, desde1, hasta1);
      servoBrazo1.write(val1);
    }

    if (i <= pasos2) {
      int val2 = map(i, 0, pasos2, desde2, hasta2);
      servoBrazo2.write(val2);
    }

    delay(10); // Puedes ajustar esto para hacerlo aún más suave o más rápido
  }
}

void setup() {
  servoBase.attach(4);
  servoBrazo1.attach(8);
  servoBrazo2.attach(12);
  Serial.begin(9600);
}

void loop() {
  if (Serial.available()) {
    char comando = Serial.read();

    switch (comando) {
      case 'a': // Base a 180°
        moverServoSuave(servoBase, basePos, 180);
        basePos = 180;
        break;

      case 'b': // Base a 0°
        moverServoSuave(servoBase, basePos, 0);
        basePos = 0;
        break;

      case 'c': // Brazo 1 y 2 a 90°
        moverBrazosSimultaneo(brazo1Pos, 90, brazo2Pos, 90);
        brazo1Pos = 90;
        brazo2Pos = 90;
        break;

      case 'd': // Brazo 1 y 2 a 0°
        moverBrazosSimultaneo(brazo1Pos, 0, brazo2Pos, 0);
        brazo1Pos = 0;
        brazo2Pos = 0;
        break;

      case 'r': // Secuencia: primero brazos, luego base
        moverBrazosSimultaneo(brazo1Pos, 0, brazo2Pos, 0);
        brazo1Pos = 0;
        brazo2Pos = 0;

        if (basePos != 0) {
          moverServoSuave(servoBase, basePos, 0);
          basePos = 0;
        }
        break;
    }

    Serial.print(basePos);
    Serial.print(",");
    Serial.print(brazo1Pos);
    Serial.print(",");
    Serial.println(brazo2Pos);
    delay(50); // actualiza cada 50 ms
  }
}
