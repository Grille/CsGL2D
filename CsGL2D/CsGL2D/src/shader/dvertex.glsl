#version 400
uniform vec2 uResolution;

in vec2 aPosition;
in vec3 aTexturePos;
in vec4 aColor;

out vec2 vPosition;
out vec3 vTexturePos;
out vec4 vColor;

void normalize2D(out vec2 oPosition, out vec3 oTexture, out vec4 oColor) {
	oPosition = ((aPosition / uResolution * 2) - 1);
	oPosition.y = -oPosition.y;

	oTexture = aTexturePos;
	oTexture.x /= 2048.0;
	oTexture.y /= 2048.0;

	oColor = aColor / 255;
}
void main(void)
{
	normalize2D(vPosition, vTexturePos, vColor);
	gl_Position = vec4(vPosition, 0, 1);
}

