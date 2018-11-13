#version 400
uniform vec2 uResolution;

in vec2 aPosition;
in vec3 aTexturePos;
in vec4 aColor;

out vec2 vPosition;
out vec3 vTexturePos;
out vec4 vColor;

void main(void)
{
	vPosition = ((aPosition / uResolution * 2) - 1);
	vPosition.y = -vPosition.y;
	vTexturePos = aTexturePos;
	vTexturePos.x /= 2048.0;
	vTexturePos.y /= 2048.0;
	vColor = aColor / 255;
	gl_Position = vec4(vPosition, 0, 1);
}